import React from 'react';
import { useTranslation } from 'react-i18next';
import { useSignalR } from '../contexts/SignalRContext';
import { useAuth } from '../contexts/AuthContext';
import { StockTicker } from './StockTicker';

export const Dashboard: React.FC = () => {
  const { t, i18n } = useTranslation();
  const { stockEvents } = useSignalR();
  const { logout } = useAuth();

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
  };

  return (
    <div style={{ width: '100%' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '2rem' }}>
        <h2>{t('dashboard.title')}</h2>
        <div>
          <button style={{ width: 'auto', marginRight: '0.5rem', padding: '0.5rem' }} onClick={() => changeLanguage('tr')}>TR</button>
          <button style={{ width: 'auto', marginRight: '0.5rem', padding: '0.5rem' }} onClick={() => changeLanguage('en')}>EN</button>
          <button style={{ width: 'auto', padding: '0.5rem', background: '#e74c3c', color: 'white' }} onClick={logout}>Çıkış</button>
        </div>
      </div>
      
      <div className="grid">
        {Object.values(stockEvents).map(stock => (
          <StockTicker key={stock.symbol} data={stock} />
        ))}
        {Object.keys(stockEvents).length === 0 && (
          <p style={{ color: '#aaa' }}>Canlı piyasa verisi bekleniyor...</p>
        )}
      </div>
    </div>
  );
};
