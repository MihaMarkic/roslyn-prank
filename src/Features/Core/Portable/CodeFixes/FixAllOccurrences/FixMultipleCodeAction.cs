// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.CodeFixes
{
    /// <summary>
    /// Fix multiple occurrences code action.
    /// </summary>
    internal partial class FixMultipleCodeAction : FixAllCodeAction
    {
        private readonly string _title;
        private readonly string _previewChangesDialogTitle;
        private readonly string _computingFixWaitDialogMessage;

        internal FixMultipleCodeAction(FixMultipleContext fixMultipleContext, FixAllProvider fixAllProvider, string title, string previewChangesDialogTitle, string computingFixWaitDialogMessage, bool showPreviewChangesDialog)
            : base (fixMultipleContext, fixAllProvider, showPreviewChangesDialog)
        {
            _title = title;
            _previewChangesDialogTitle = previewChangesDialogTitle;
            _computingFixWaitDialogMessage = computingFixWaitDialogMessage;
        }

        public Diagnostic GetTriggerDiagnostic()
        {
            return ((FixMultipleContext)FixAllContext).GetTriggerDiagnostic();
        }

        public override string Title => _title;
        protected override string FixAllPreviewChangesTitle => _previewChangesDialogTitle;
        protected override string ComputingFixAllWaitDialogMessage => _computingFixWaitDialogMessage;
    }
}
