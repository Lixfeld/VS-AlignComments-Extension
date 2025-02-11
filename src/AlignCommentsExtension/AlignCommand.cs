﻿using System;
using System.ComponentModel.Design;
using System.Linq;
using AlignCommentsExtension.Classes;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using static AlignCommentsExtension.Classes.Constants;
using Task = System.Threading.Tasks.Task;

namespace AlignCommentsExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AlignCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("a42f7e31-20cf-4356-9812-2793c1534e36");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// The top-level object in the Visual Studio automation object model
        /// </summary>
        private static DTE2 dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AlignCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AlignCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in AlignCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AlignCommand(package, commandService);

            // Get DTE object
            if (await package.GetServiceAsync(typeof(DTE)) is DTE2 dte2)
                dte = dte2;
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IWpfTextView textView = TextViewHelper.GetActiveTextView();
            (int startLineNo, int endLineNo) = TextViewHelper.GetSelectedLineNumbers(textView);
            SelectedLines selectedLines = new SelectedLines(textView.TextSnapshot, startLineNo, endLineNo);

            if (selectedLines.Lines.Count() >= 2)
            {
                int tabSize = dte.ActiveDocument.TabSize;
                string delimiter = GetCommentDelimiter();

                CommentAligner commentAligner = new CommentAligner(selectedLines.Lines, tabSize, selectedLines.LineEnding, delimiter);
                string newText = commentAligner.GetText();

                try
                {
                    dte.UndoContext.Open("Align comments");
                    TextViewHelper.ReplaceText(textView, selectedLines.StartPosition, selectedLines.Length, newText);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }
                finally
                {
                    dte.UndoContext.Close();
                }
            }
        }

        private string GetCommentDelimiter()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string name = dte.ActiveDocument.Name;
            if (name.EndsWith(VisualBasicExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                return Apostrophe;
            }
            return DoubleSlash;
        }
    }
}
