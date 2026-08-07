import React, { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuth } from './AuthContext';

export interface AnalysisInfoPublishedEvent {
  analysisId: string;
  symbol: string;
  recommendation: string;
  score: number;
  publishedAt: string;
}

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  stockEvents: Record<string, AnalysisInfoPublishedEvent>;
  signalHistory: AnalysisInfoPublishedEvent[];
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined);

export const SignalRProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const { token } = useAuth();
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [stockEvents, setStockEvents] = useState<Record<string, AnalysisInfoPublishedEvent>>({});
  const [signalHistory, setSignalHistory] = useState<AnalysisInfoPublishedEvent[]>([]);

  useEffect(() => {
    if (!token) {
      if (connection) {
        connection.stop();
        setConnection(null);
      }
      return;
    }

    const baseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000';
    const hubUrl = `${baseUrl}/hubs/stock`; 
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    newConnection.start().catch(err => console.error('SignalR Connection Error: ', err));

    newConnection.on('ReceiveStockUpdate', (data: AnalysisInfoPublishedEvent) => {
      // Update latest state per symbol
      setStockEvents(prev => ({
        ...prev,
        [data.symbol]: data
      }));
      
      // Update signal history log (keep last 50 events)
      setSignalHistory(prev => {
        const newHistory = [data, ...prev];
        return newHistory.slice(0, 50);
      });
    });

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, [token]);

  return (
    <SignalRContext.Provider value={{ connection, stockEvents, signalHistory }}>
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
