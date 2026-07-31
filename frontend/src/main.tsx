import React from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import './i18n';
import { AuthProvider } from './contexts/AuthContext';
import { SignalRProvider } from './contexts/SignalRContext';

const rootElement = document.getElementById('root');
if (rootElement) {
  createRoot(rootElement).render(
    <React.StrictMode>
      <AuthProvider>
        <SignalRProvider>
          <div className="container">
            <App />
          </div>
        </SignalRProvider>
      </AuthProvider>
    </React.StrictMode>
  );
}
