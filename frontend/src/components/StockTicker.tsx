import React, { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { StockPriceAnalyzedEvent } from '../contexts/SignalRContext';

interface Props {
  data: StockPriceAnalyzedEvent;
}

export const StockTicker: React.FC<Props> = ({ data }) => {
  const { t } = useTranslation();
  const prevPriceRef = useRef(data.price);
  const [flashClass, setFlashClass] = useState('');

  useEffect(() => {
    if (data.price > prevPriceRef.current) {
      setFlashClass('flash-up');
    } else if (data.price < prevPriceRef.current) {
      setFlashClass('flash-down');
    }
    
    prevPriceRef.current = data.price;
    
    const timer = setTimeout(() => setFlashClass(''), 1000);
    return () => clearTimeout(timer);
  }, [data.price, data.timestamp]);

  const getSignalClass = (signal: number) => {
    switch (signal) {
      case 1: return 'signal-buy';
      case 2: return 'signal-sell';
      default: return 'signal-hold';
    }
  };

  return (
    <div className={`ticker-card ${flashClass}`}>
      <div className="ticker-symbol">{data.symbol}</div>
      <div className="ticker-price">${data.price.toFixed(2)}</div>
      
      <div>
        <span className={`signal ${getSignalClass(data.signal)}`}>
          {t(`dashboard.signals.${data.signal}`)}
        </span>
      </div>
      {data.aiReason && (
        <div style={{ marginTop: '0.5rem', fontSize: '0.8rem', color: '#bb86fc' }}>
          {data.aiReason}
        </div>
      )}
      <div style={{ fontSize: '0.7rem', color: '#777', marginTop: '0.5rem' }}>
        {new Date(data.timestamp).toLocaleTimeString()}
      </div>
    </div>
  );
};
