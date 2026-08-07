import React, { useEffect, useRef, useState } from 'react';

import type { AnalysisInfoPublishedEvent } from '../contexts/SignalRContext';

interface Props {
  data: AnalysisInfoPublishedEvent;
}

export const StockTicker: React.FC<Props> = ({ data }) => {
  const prevScoreRef = useRef(data.score);
  const [flashClass, setFlashClass] = useState('');

  useEffect(() => {
    if (data.score > prevScoreRef.current) {
      setFlashClass('flash-up');
    } else if (data.score < prevScoreRef.current) {
      setFlashClass('flash-down');
    }
    
    prevScoreRef.current = data.score;
    
    const timer = setTimeout(() => setFlashClass(''), 1000);
    return () => clearTimeout(timer);
  }, [data.score, data.publishedAt]);

  const getSignalClass = (recommendation: string) => {
    switch (recommendation.toLowerCase()) {
      case 'buy': return 'signal-buy';
      case 'sell': return 'signal-sell';
      default: return 'signal-hold';
    }
  };

  return (
    <div className={`ticker-card ${flashClass}`}>
      <div className="ticker-symbol">{data.symbol}</div>
      <div className="ticker-price">Score: {data.score.toFixed(2)}</div>
      
      <div>
        <span className={`signal ${getSignalClass(data.recommendation)}`}>
          {data.recommendation}
        </span>
      </div>
      <div style={{ fontSize: '0.7rem', color: '#777', marginTop: '0.5rem' }}>
        {new Date(data.publishedAt).toLocaleTimeString()}
      </div>
    </div>
  );
};
