import React from 'react';
import { useSignalR } from '../contexts/SignalRContext';

export const LiveOrdersFeed: React.FC = () => {
  const { recentOrders } = useSignalR();

  const getSideBadge = (side: string) => {
    switch (side.toLowerCase()) {
      case 'buy':
        return <div className="bullish-bg bullish-text font-label-sm text-[10px] px-2 py-0.5 rounded border border-primary/20">BUY</div>;
      case 'sell':
        return <div className="bearish-bg bearish-text font-label-sm text-[10px] px-2 py-0.5 rounded border border-error/20">SELL</div>;
      default:
        return <div className="bg-white/10 text-on-surface-variant font-label-sm text-[10px] px-2 py-0.5 rounded">{side.toUpperCase()}</div>;
    }
  };

  const getRowStyle = (side: string) => {
    if (side.toLowerCase() === 'buy') return 'border-white/5 hover:border-primary/30';
    if (side.toLowerCase() === 'sell') return 'border-white/5 hover:border-error/30';
    return 'border-white/5 opacity-60 grayscale hover:grayscale-0';
  };

  return (
    <div className="flex-1 glass-panel rounded-xl flex flex-col min-h-[400px] lg:min-h-0 overflow-hidden border-t-2 border-t-primary/50">
      {/* Feed Header */}
      <div className="p-md border-b border-white/5 bg-gradient-to-r from-primary/10 to-transparent shrink-0 flex items-center justify-between">
        <div className="flex items-center gap-2">
          <span className="material-symbols-outlined text-primary animate-pulse" style={{fontVariationSettings: "'FILL' 1"}}>swap_horiz</span>
          <h3 className="font-headline-md text-[16px] text-on-surface m-0">Live Order Feed</h3>
        </div>
        <span className="relative flex h-3 w-3">
          <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-primary opacity-75"></span>
          <span className="relative inline-flex rounded-full h-3 w-3 bg-primary"></span>
        </span>
      </div>

      {/* Feed List */}
      <div className="flex-1 overflow-y-auto p-2 space-y-2 relative">
        {recentOrders.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-on-surface-variant/50 p-4 text-center">
            <span className="material-symbols-outlined text-[48px] mb-2 opacity-20">hourglass_empty</span>
            <p className="font-data-md text-sm">Waiting for incoming orders...</p>
          </div>
        ) : (
          recentOrders.map((order, idx) => (
            <div key={`${order.orderId}-${idx}`} className={`bg-surface-container/50 border p-3 rounded-lg transition-all cursor-pointer group ${getRowStyle(order.side)}`}>
              <div className="flex justify-between items-start mb-2">
                <div className="flex items-center gap-2">
                  <span className="font-data-lg text-on-surface text-[16px]">{order.symbol}</span>
                  <span className="font-data-md text-on-surface-variant text-[11px]">
                    {new Date(order.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second:'2-digit' })}
                  </span>
                </div>
                {getSideBadge(order.side)}
              </div>
              <div className="flex justify-between items-end">
                <span className="font-data-md text-[14px]">
                  Price: ${order.price.toFixed(2)}
                </span>
                <div className="flex items-center gap-2">
                  <span className="font-label-sm text-on-surface-variant text-[10px]">QTY</span>
                  <span className="font-data-md text-[12px] text-on-surface font-bold">
                    {order.quantity}
                  </span>
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
