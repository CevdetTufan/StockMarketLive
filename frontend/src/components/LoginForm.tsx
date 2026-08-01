import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';

export const LoginForm: React.FC = () => {
  const { t } = useTranslation();
  const { login } = useAuth();
  
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Uygulama Mimari Kararı: API login entegrasyonu (Mock Endpoint)
    try {
      const response = await fetch('http://localhost:5000/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
      });
      
      if (response.ok) {
        const data = await response.json();
        login(data.token);
      } else {
        setError(true);
      }
    } catch {
      setError(true);
    }
  };

  return (
    <div className="min-h-screen bg-background flex flex-col justify-center items-center p-4">
      {/* Background Decorative Elements */}
      <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-primary/10 rounded-full blur-3xl pointer-events-none"></div>
      <div className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-tertiary/10 rounded-full blur-3xl pointer-events-none"></div>
      
      {/* Brand Header */}
      <div className="mb-8 text-center z-10">
        <h1 className="font-display-lg text-display-lg text-primary tracking-tighter text-4xl md:text-5xl font-bold mb-2">
          StockMarket Live
        </h1>
        <p className="text-on-surface-variant font-data-md">
          <span className="material-symbols-outlined text-[14px] align-middle mr-1 text-tertiary">psychology</span>
          Quantum AI Trading Engine
        </p>
      </div>

      {/* Login Card */}
      <div className="glass-panel w-full max-w-md p-8 rounded-2xl z-10 shadow-[0_0_40px_rgba(0,0,0,0.5)] border border-white/10">
        <h2 className="font-headline-md text-2xl text-on-surface mb-6 text-center">{t('login.title') || 'Terminal Access'}</h2>
        
        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label className="block font-label-sm text-on-surface-variant mb-1 uppercase tracking-wider text-[11px]">
              {t('login.username') || 'Username'}
            </label>
            <div className="relative">
              <span className="absolute left-3 top-1/2 -translate-y-1/2 material-symbols-outlined text-on-surface-variant text-[18px]">person</span>
              <input 
                type="text" 
                className="w-full bg-surface-container-highest border border-white/10 rounded-lg py-3 pl-10 pr-4 text-on-surface focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all font-data-md"
                placeholder="Enter your username"
                value={username}
                onChange={e => setUsername(e.target.value)}
                required
              />
            </div>
          </div>
          
          <div>
            <label className="block font-label-sm text-on-surface-variant mb-1 uppercase tracking-wider text-[11px]">
              {t('login.password') || 'Password'}
            </label>
            <div className="relative">
              <span className="absolute left-3 top-1/2 -translate-y-1/2 material-symbols-outlined text-on-surface-variant text-[18px]">lock</span>
              <input 
                type="password" 
                className="w-full bg-surface-container-highest border border-white/10 rounded-lg py-3 pl-10 pr-4 text-on-surface focus:outline-none focus:border-primary/50 focus:ring-1 focus:ring-primary/50 transition-all font-data-md"
                placeholder="••••••••"
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
              />
            </div>
          </div>

          {error && (
            <div className="bg-error-container/20 border border-error/50 rounded-lg p-3 flex items-start gap-2">
              <span className="material-symbols-outlined text-error text-[18px]">error</span>
              <p className="text-error text-sm font-data-md">{t('login.error') || 'Invalid credentials'}</p>
            </div>
          )}
          
          <button 
            type="submit"
            className="w-full bg-primary hover:bg-primary-fixed text-on-primary font-bold py-3 rounded-lg transition-all active:scale-[0.98] shadow-[0_0_20px_rgba(68,224,146,0.2)] hover:shadow-[0_0_25px_rgba(68,224,146,0.4)] flex justify-center items-center gap-2 mt-4"
          >
            {t('login.submit') || 'Authenticate'}
            <span className="material-symbols-outlined text-[18px]">login</span>
          </button>
        </form>
      </div>
      
      {/* Footer Info */}
      <div className="mt-8 text-on-surface-variant/50 font-data-md text-xs text-center z-10 flex items-center gap-1">
        <span className="material-symbols-outlined text-[14px]">lock</span>
        Secure End-to-End Encryption Enabled
      </div>
    </div>
  );
};
