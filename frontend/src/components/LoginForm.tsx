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
    <div className="panel">
      <h2>{t('login.title')}</h2>
      <form onSubmit={handleSubmit}>
        <input 
          type="text" 
          placeholder={t('login.username')}
          value={username}
          onChange={e => setUsername(e.target.value)}
          required
        />
        <input 
          type="password" 
          placeholder={t('login.password')}
          value={password}
          onChange={e => setPassword(e.target.value)}
          required
        />
        {error && <p style={{color: '#e74c3c'}}>{t('login.error')}</p>}
        <button type="submit">{t('login.submit')}</button>
      </form>
    </div>
  );
};
