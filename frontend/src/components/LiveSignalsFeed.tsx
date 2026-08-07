import React from 'react';
import { useSignalR } from '../contexts/SignalRContext';

export const LiveSignalsFeed: React.FC = () => {
  const { signalHistory } = useSignalR();

  const getSignalBadge = (recommendation: string) => {
    switch (recommendation.toLowerCase()) {
      case 'buy':
        return <div className="bullish-bg bullish-text font-label-sm text-[10px] px-2 py-0.5 rounded border border-primary/20">BUY</div>;
      case 'sell':
        return <div className="bearish-bg bearish-text font-label-sm text-[10px] px-2 py-0.5 rounded border border-error/20">SELL</div>;
      default:
        return <div className="bg-white/10 text-on-surface-variant font-label-sm text-[10px] px-2 py-0.5 rounded">HOLD</div>;
    }
  };

  const getSignalRowStyle = (recommendation: string) => {
    if (recommendation.toLowerCase() === 'buy') return 'border-white/5 hover:border-primary/30';
    if (recommendation.toLowerCase() === 'sell') return 'border-white/5 hover:border-error/30';
    return 'border-white/5 opacity-60 grayscale hover:grayscale-0';
  };

  return (
    <div className="flex-1 glass-panel rounded-xl flex flex-col min-h-[400px] lg:min-h-0 overflow-hidden border-t-2 border-t-tertiary/50">
      {/* Feed Header */}
      <div className="p-md border-b border-white/5 bg-gradient-to-r from-tertiary/10 to-transparent shrink-0 flex items-center justify-between">
        <div className="flex items-center gap-2">
          <span className="material-symbols-outlined text-tertiary animate-pulse" style={{fontVariationSettings: "'FILL' 1"}}>radar</span>
          <h3 className="font-headline-md text-[16px] text-on-surface m-0">Live AI Signals</h3>
        </div>
        <span className="relative flex h-3 w-3">
          <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-tertiary opacity-75"></span>
          <span className="relative inline-flex rounded-full h-3 w-3 bg-tertiary"></span>
        </span>
      </div>

      {/* Feed List */}
      <div className="flex-1 overflow-y-auto p-2 space-y-2 relative">
        {signalHistory.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-on-surface-variant/50 p-4 text-center">
            <span className="material-symbols-outlined text-[48px] mb-2 opacity-20">hourglass_empty</span>
            <p className="font-data-md text-sm">Waiting for incoming AI signals...</p>
          </div>
        ) : (
          signalHistory.map((event, idx) => (
            <div key={`${event.analysisId}-${idx}`} className={`bg-surface-container/50 border p-3 rounded-lg transition-all cursor-pointer group ${getSignalRowStyle(event.recommendation)}`}>
              <div className="flex justify-between items-start mb-2">
                <div className="flex items-center gap-2">
                  <span className="font-data-lg text-on-surface text-[16px]">{event.symbol}</span>
                  <span className="font-data-md text-on-surface-variant text-[11px]">
                    {new Date(event.publishedAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second:'2-digit' })}
                  </span>
                </div>
                {getSignalBadge(event.recommendation)}
              </div>
              <div className="flex justify-between items-end">
                <span className="font-data-md text-[14px]">
                  Score: {event.score.toFixed(2)}
                </span>
                <div className="flex items-center gap-2">
                  <span className="font-label-sm text-tertiary text-[10px]">AI SCORE</span>
                  <div className="relative w-6 h-6 rounded-full flex items-center justify-center bg-tertiary/10 border border-tertiary/30">
                    <span className="font-data-md text-[9px] text-tertiary font-bold">
                      {Math.round(event.score)}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          ))
        )}
        
        {/* Fading gradient at bottom of list */}
        <div className="sticky bottom-0 left-0 right-0 h-12 bg-gradient-to-t from-surface-container-lowest to-transparent pointer-events-none"></div>
      </div>
    </div>
  );
};
