import React from 'react';
import { useAuth } from './contexts/AuthContext';
import { LoginForm } from './components/LoginForm';
import { Dashboard } from './components/Dashboard';
import './index.css';

const AppContent: React.FC = () => {
  const { token } = useAuth();
  return token ? <Dashboard /> : <LoginForm />;
};

export default AppContent;
