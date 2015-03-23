// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Editor.Host;
using Microsoft.CodeAnalysis.Editor.Shared.Extensions;
using Microsoft.CodeAnalysis.Editor.Shared.Options;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;

namespace Microsoft.CodeAnalysis.Editor.Implementation.RenameTracking
{
    internal sealed partial class RenameTrackingTaggerProvider
    {
        private class Tagger : ITagger<RenameTrackingTag>, IDisposable
        {
            private readonly StateMachine _stateMachine;
            private readonly ITextUndoHistoryRegistry _undoHistoryRegistry;
            private readonly IWaitIndicator _waitIndicator;
            private readonly IEnumerable<IRefactorNotifyService> _refactorNotifyServices;

            public event EventHandler<SnapshotSpanEventArgs> TagsChanged = delegate { };

            public Tagger(
                StateMachine stateMachine,
                ITextUndoHistoryRegistry undoHistoryRegistry,
                IWaitIndicator waitIndicator,
                IEnumerable<IRefactorNotifyService> refactorNotifyServices)
            {
                _stateMachine = stateMachine;
                _stateMachine.Connect();
                _stateMachine.TrackingSessionUpdated += StateMachine_TrackingSessionUpdated;
                _stateMachine.TrackingSessionCleared += StateMachine_TrackingSessionCleared;
                _undoHistoryRegistry = undoHistoryRegistry;
                _waitIndicator = waitIndicator;
                _refactorNotifyServices = refactorNotifyServices;
            }

            private void StateMachine_TrackingSessionCleared(ITrackingSpan trackingSpanToClear)
            {
                TagsChanged(this, new SnapshotSpanEventArgs(trackingSpanToClear.GetSpan(_stateMachine.Buffer.CurrentSnapshot)));
            }

            private void StateMachine_TrackingSessionUpdated()
            {
                if (_stateMachine.TrackingSession != null)
                {
                    TagsChanged(this, new SnapshotSpanEventArgs(_stateMachine.TrackingSession.TrackingSpan.GetSpan(_stateMachine.Buffer.CurrentSnapshot)));
                }
            }

            public IEnumerable<ITagSpan<RenameTrackingTag>> GetTags(NormalizedSnapshotSpanCollection spans)
            {
                if (!_stateMachine.Buffer.GetOption(InternalFeatureOnOffOptions.RenameTracking))
                {
                    // Changes aren't being triggered by the buffer, but there may still be taggers
                    // out there which we should prevent from doing work.
                    yield break;
                }

                TrackingSession trackingSession;
                if (_stateMachine.CanInvokeRename(out trackingSession, isSmartTagCheck: true))
                {
                    foreach (var span in spans)
                    {
                        var snapshotSpan = trackingSession.TrackingSpan.GetSpan(span.Snapshot);
                        if (span.IntersectsWith(snapshotSpan))
                        {
                            yield return new TagSpan<RenameTrackingTag>(snapshotSpan, RenameTrackingTag.Instance);
                        }
                    }
                }
            }

            public void Dispose()
            {
                _stateMachine.TrackingSessionUpdated -= StateMachine_TrackingSessionUpdated;
                _stateMachine.TrackingSessionCleared -= StateMachine_TrackingSessionCleared;
                _stateMachine.Disconnect();
            }
        }
    }
}