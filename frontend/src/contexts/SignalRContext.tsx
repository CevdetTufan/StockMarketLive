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

export interface OrderCreatedEvent {
  orderId: string;
  symbol: string;
  price: number;
  quantity: number;
  side: string;
  createdAt: string;
}

export interface StockPriceUpdatedEvent {
  symbol: string;
  currentPrice: number;
  changeRate: number;
  updatedAt: string;
}

interface SignalRContextType {
  connection: signalR.HubConnection | null;
  stockEvents: Record<string, AnalysisInfoPublishedEvent>;
  signalHistory: AnalysisInfoPublishedEvent[];
  livePrices: Record<string, StockPriceUpdatedEvent>;
  recentOrders: OrderCreatedEvent[];
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined);

export const SignalRProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const { token } = useAuth();
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [stockEvents, setStockEvents] = useState<Record<string, AnalysisInfoPublishedEvent>>({});
  const [signalHistory, setSignalHistory] = useState<AnalysisInfoPublishedEvent[]>([]);
  const [livePrices, setLivePrices] = useState<Record<string, StockPriceUpdatedEvent>>({});
  const [recentOrders, setRecentOrders] = useState<OrderCreatedEvent[]>([]);

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
      console.log('[SignalR] ReceiveStockUpdate:', data);
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

    newConnection.on('ReceiveStockPriceUpdated', (data: StockPriceUpdatedEvent) => {
      console.log('[SignalR] ReceiveStockPriceUpdated:', data);
      setLivePrices(prev => ({
        ...prev,
        [data.symbol]: data
      }));
    });

    newConnection.on('ReceiveOrderCreated', (data: OrderCreatedEvent) => {
      console.log('[SignalR] ReceiveOrderCreated:', data);
      setRecentOrders(prev => {
        const newOrders = [data, ...prev];
        return newOrders.slice(0, 20); // Keep last 20 orders
      });
    });

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, [token]);

  return (
    <SignalRContext.Provider value={{ connection, stockEvents, signalHistory, livePrices, recentOrders }}>
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
