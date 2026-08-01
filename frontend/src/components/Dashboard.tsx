import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';
import { useSignalR } from '../contexts/SignalRContext';
import { LiveSignalsFeed } from './LiveSignalsFeed';
import { UserManagement } from './UserManagement';

export const Dashboard: React.FC = () => {
  const { t, i18n } = useTranslation();
  const { logout, isAdmin } = useAuth();
  const { stockEvents } = useSignalR();
  const [selectedSymbol, setSelectedSymbol] = useState<string>('AAPL');
  const [activeTab, setActiveTab] = useState<'terminal' | 'users'>('terminal');

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
  };

  // Hesaplamalar (Mock Portfolio Verileri İçin)
  const activePositionsCount = Object.keys(stockEvents).length;
  
  // Bulunan en son stock event bilgisini almak için
  const selectedStockData = stockEvents[selectedSymbol] || Object.values(stockEvents)[0] || null;

  return (
    <div className="bg-background text-on-surface font-body-base antialiased overflow-hidden selection:bg-primary/30 selection:text-primary h-screen w-full flex flex-col">
      {/* TopNavBar */}
      <nav className="bg-background/80 backdrop-blur-xl border-b border-white/10 shadow-[0_0_20px_rgba(68,224,146,0.1)] flex justify-between items-center px-margin h-16 w-full z-50 shrink-0">
        <div className="flex items-center gap-margin">
          <div className="font-display-lg text-display-lg text-primary tracking-tighter text-[28px] font-bold">StockMarket Live</div>
          <div className="hidden md:flex items-center glass-panel rounded-full px-4 py-2 w-64 focus-within:border-primary/50 transition-colors">
            <span className="material-symbols-outlined text-on-surface-variant text-[20px] mr-2">search</span>
            <input 
              type="text" 
              className="bg-transparent border-none outline-none text-data-md text-on-surface w-full placeholder:text-on-surface-variant/50 focus:ring-0 p-0" 
              placeholder="Search ticker..." 
              value={selectedSymbol}
              onChange={(e) => setSelectedSymbol(e.target.value.toUpperCase())}
            />
          </div>
        </div>
        
        <div className="flex items-center gap-md">
          <button onClick={() => changeLanguage(i18n.language === 'tr' ? 'en' : 'tr')} className="text-on-surface-variant hover:text-primary font-label-sm px-2">
            {i18n.language === 'tr' ? 'EN' : 'TR'}
          </button>
          <button className="bg-primary text-on-primary font-label-sm px-4 py-2 rounded-md hover:bg-primary-fixed transition-colors active:scale-95 ml-2 font-bold shadow-[0_0_15px_rgba(68,224,146,0.3)]">
            Execute Trade
          </button>
          <button onClick={logout} className="ml-4 p-2 text-error hover:text-error-container transition-colors duration-200" title="Sign Out">
            <span className="material-symbols-outlined">logout</span>
          </button>
        </div>
      </nav>

      <div className="flex flex-1 overflow-hidden">
        {/* SideNavBar */}
        <aside className="bg-surface-container-lowest/90 backdrop-blur-lg border-r border-white/5 w-64 flex-col py-md z-40 hidden md:flex h-full">
          <div className="px-margin mb-8 flex items-center gap-3">
            <div className="h-10 w-10 rounded-lg border border-tertiary/30 bg-tertiary/10 flex items-center justify-center shrink-0">
              <span className="material-symbols-outlined text-tertiary" style={{fontVariationSettings: "'FILL' 1"}}>psychology</span>
            </div>
            <div>
              <div className="font-headline-md text-[18px] text-primary font-semibold leading-tight">Quantum Engine</div>
              <div className="font-label-sm text-[10px] text-tertiary uppercase tracking-wider opacity-80 mt-1 flex items-center gap-1">
                <span className="w-1.5 h-1.5 rounded-full bg-tertiary animate-pulse"></span>
                Processing Real-time
              </div>
            </div>
          </div>

          <div className="flex-1 flex flex-col gap-1 px-2">
            <a href="#" onClick={(e) => { e.preventDefault(); setActiveTab('terminal'); }} className={`px-4 py-3 flex items-center gap-md rounded-l-md transition-all duration-150 group ${activeTab === 'terminal' ? 'text-primary bg-primary/10 border-r-4 border-primary active:translate-x-1' : 'text-on-surface-variant hover:bg-white/5 hover:text-on-surface active:translate-x-1'}`}>
              <span className="material-symbols-outlined group-hover:scale-110 transition-transform">dashboard</span>
              <span className="font-label-sm text-[14px]">{t('dashboard.title') || 'Terminal'}</span>
            </a>
            <a href="#" className="text-on-surface-variant px-4 py-3 flex items-center gap-md hover:bg-white/5 hover:text-on-surface transition-all rounded-l-md active:translate-x-1 duration-150 group">
              <span className="material-symbols-outlined group-hover:scale-110 transition-transform">auto_graph</span>
              <span className="font-label-sm text-[14px]">Signals</span>
            </a>
            <a href="#" className="text-on-surface-variant px-4 py-3 flex items-center gap-md hover:bg-white/5 hover:text-on-surface transition-all rounded-l-md active:translate-x-1 duration-150 group">
              <span className="material-symbols-outlined group-hover:scale-110 transition-transform">account_balance_wallet</span>
              <span className="font-label-sm text-[14px]">Portfolio</span>
            </a>
            <a href="#" className="flex items-center gap-3 px-4 py-3 rounded-xl transition-all text-white/70 hover:bg-white/10 hover:text-white">
              <span className="text-xl">⚙️</span>
              <span className="font-medium text-sm">Ayarlar</span>
            </a>
            
            {isAdmin && (
              <a href="#" onClick={(e) => { e.preventDefault(); setActiveTab('users'); }} className={`flex items-center gap-3 px-4 py-3 rounded-xl transition-all ${activeTab === 'users' ? 'bg-emerald-500/20 text-emerald-300 border-r-4 border-emerald-400' : 'bg-emerald-500/10 text-emerald-400 hover:bg-emerald-500/20 border border-emerald-500/20'}`}>
                <span className="text-xl">🛡️</span>
                <span className="font-medium text-sm">Kullanıcı Yönetimi</span>
              </a>
            )}
          </div>
        </aside>

        {/* Main Content Area */}
        <main className="flex-1 flex flex-col bg-surface-container-lowest overflow-hidden">
          {activeTab === 'terminal' ? (
            <>
              {/* Top Widgets Row */}
              <div className="p-gutter grid grid-cols-1 md:grid-cols-3 gap-gutter shrink-0">
                {/* Widget 1 */}
                <div className="glass-panel rounded-xl p-md flex flex-col justify-between h-24 relative overflow-hidden">
                  <div className="absolute top-0 right-0 p-3 opacity-10">
                    <span className="material-symbols-outlined text-[64px] text-primary">account_balance_wallet</span>
                  </div>
                  <div className="font-label-sm text-on-surface-variant uppercase tracking-widest text-[10px]">Total P&L (Live)</div>
                  <div className="flex items-end justify-between">
                    <div className="font-data-lg text-[28px] text-primary font-semibold">+₺12,450.00</div>
                    <div className="bullish-bg bullish-text font-data-md text-[12px] px-2 py-0.5 rounded flex items-center gap-1 mb-1">
                      <span className="material-symbols-outlined text-[14px]">arrow_upward</span> 2.4%
                    </div>
                  </div>
                </div>
                
                {/* Widget 2 */}
                <div className="glass-panel rounded-xl p-md flex flex-col justify-between h-24 relative overflow-hidden">
                  <div className="absolute top-0 right-0 p-3 opacity-10">
                    <span className="material-symbols-outlined text-[64px] text-on-surface">monitoring</span>
                  </div>
                  <div className="font-label-sm text-on-surface-variant uppercase tracking-widest text-[10px]">Active AI Tracked</div>
                  <div className="flex items-end justify-between">
                    <div className="font-data-lg text-[28px] text-on-surface font-semibold">{activePositionsCount}</div>
                    <div className="text-on-surface-variant font-data-md text-[12px] mb-1">
                      Symbols Streaming
                    </div>
                  </div>
                </div>

                {/* Widget 3 */}
                <div className="glass-panel rounded-xl p-md flex flex-col justify-between h-24 relative overflow-hidden ai-glow border-tertiary/20">
                  <div className="absolute top-0 right-0 p-3 opacity-10">
                    <span className="material-symbols-outlined text-[64px] text-tertiary">psychology</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <div className="font-label-sm text-tertiary uppercase tracking-widest text-[10px] flex items-center gap-1">
                      <span className="material-symbols-outlined text-[14px]">smart_toy</span>
                      AI Market Sentiment
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-full border-2 border-primary border-t-transparent animate-spin flex items-center justify-center shrink-0">
                      <span className="material-symbols-outlined text-primary text-[20px] animate-none">trending_up</span>
                    </div>
                    <div>
                      <div className="font-data-lg text-[20px] text-primary font-semibold">Bullish</div>
                      <div className="font-data-md text-on-surface-variant text-[12px]">72% Confidence Level</div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Layout Split: Chart & Feed */}
              <div className="flex-1 flex flex-col lg:flex-row gap-gutter px-gutter pb-gutter min-h-0 overflow-hidden">
                {/* Main Chart Area */}
                <div className="flex-[3] glass-panel rounded-xl flex flex-col min-h-[400px] lg:min-h-0 relative overflow-hidden">
                  <div className="p-md border-b border-white/5 flex justify-between items-center shrink-0">
                    <div className="flex items-center gap-4">
                      <div className="flex items-baseline gap-2">
                        <h2 className="font-headline-md text-on-surface m-0 leading-none">{selectedStockData?.symbol || 'WAITING'}</h2>
                      </div>
                      <div className="h-4 w-px bg-white/20"></div>
                      <div className="font-data-lg text-on-surface">${selectedStockData?.price?.toFixed(2) || '0.00'}</div>
                    </div>
                    <div className="flex gap-2">
                      <button className="bg-surface-container-highest hover:bg-surface-bright text-on-surface font-label-sm px-3 py-1 rounded transition-colors text-[11px]">1H</button>
                      <button className="bg-primary/20 text-primary border border-primary/30 font-label-sm px-3 py-1 rounded transition-colors text-[11px]">4H</button>
                      <button className="bg-surface-container-highest hover:bg-surface-bright text-on-surface font-label-sm px-3 py-1 rounded transition-colors text-[11px]">1D</button>
                    </div>
                  </div>
                  
                  {/* Chart Canvas (CSS Placeholder based on user HTML) */}
                  <div className="flex-1 relative p-4 flex flex-col justify-center items-center" style={{ backgroundImage: 'linear-gradient(rgba(255, 255, 255, 0.03) 1px, transparent 1px), linear-gradient(90deg, rgba(255, 255, 255, 0.03) 1px, transparent 1px)', backgroundSize: '40px 40px', backgroundPosition: 'center center' }}>
                     {!selectedStockData ? (
                        <p className="text-on-surface-variant/50">Canlı fiyat bekleniyor...</p>
                     ) : (
                        <>
                          {/* Fake Chart Lines for aesthetic from HTML */}
                          <svg className="absolute inset-0 w-full h-full z-0 pointer-events-none" preserveAspectRatio="none">
                            <path className="opacity-50" d="M 50 200 Q 150 250 250 150 T 450 100 T 650 50" fill="none" stroke="#ccbdff" strokeDasharray="4 4" strokeWidth="2"></path>
                            <circle className="animate-pulse" cx="650" cy="50" fill="#ccbdff" r="4"></circle>
                          </svg>
                          <div className="font-display-lg text-primary text-6xl shadow-black drop-shadow-2xl opacity-50 absolute">
                            ${selectedStockData.price.toFixed(2)}
                          </div>
                        </>
                     )}
                  </div>
                </div>

                {/* Live AI Signals Feed */}
                <LiveSignalsFeed />
              </div>
            </>
          ) : (
            <UserManagement />
          )}
        </main>
      </div>
    </div>
  );
};
