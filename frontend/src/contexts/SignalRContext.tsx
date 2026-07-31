import React, { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuth } from './AuthContext';

export interface StockPriceAnalyzedEvent {
  symbol: string;
  price: number;
  signal: number;
  aiReason: string | null;
  timestamp: string;
}

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  stockEvents: Record<string, StockPriceAnalyzedEvent>;
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined);

export const SignalRProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const { token } = useAuth();
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [stockEvents, setStockEvents] = useState<Record<string, StockPriceAnalyzedEvent>>({});

  useEffect(() => {
    if (!token) {
      if (connection) {
        connection.stop();
        setConnection(null);
      }
      return;
    }

    // Sabit (Constant) url kuralına uyularak ortam değişkeninden alınabilir
    const hubUrl = 'http://localhost:5000/hubs/stock'; 
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    newConnection.start().catch(err => console.error('SignalR Connection Error: ', err));

    newConnection.on('ReceiveStockUpdate', (data: StockPriceAnalyzedEvent) => {
      setStockEvents(prev => ({
        ...prev,
        [data.symbol]: data
      }));
    });

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, [token]);

  return (
    <SignalRContext.Provider value={{ connection, stockEvents }}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = () => {
  const context = useContext(SignalRContext);
  if (context === undefined) {
    throw new Error('useSignalR must be used within a SignalRProvider');
  }
  return context;
};
