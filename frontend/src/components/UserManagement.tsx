import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';

interface UserDto {
  id: string;
  username: string;
  email: string;
  isAdmin: boolean;
}

export const UserManagement: React.FC = () => {
  const { token } = useAuth();
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Form State
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/users`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (res.ok) {
        const data = await res.json();
        setUsers(data);
      } else {
        setError('Kullanıcılar alınırken hata oluştu.');
      }
    } catch (err) {
      setError('Sunucu bağlantı hatası.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (token) {
      fetchUsers();
    }
  }, [token]);

  const handleAddUser = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ username, email, password })
      });

      if (res.ok) {
        setShowModal(false);
        setUsername('');
        setEmail('');
        setPassword('');
        fetchUsers(); // Refresh list
      } else {
        const errData = await res.json();
        setError(errData.error || 'Kullanıcı eklenemedi.');
      }
    } catch (err) {
      setError('Bağlantı hatası.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="flex-1 flex flex-col p-gutter h-full overflow-hidden animate-in fade-in duration-300">
      <div className="flex justify-between items-center mb-md">
        <div>
          <h2 className="font-display-md text-on-surface text-[24px] font-bold">Kullanıcı Yönetimi</h2>
          <p className="text-on-surface-variant font-body-sm">Sisteme kayıtlı kullanıcıları ve yetkilerini buradan yönetebilirsiniz.</p>
        </div>
        <button 
          onClick={() => setShowModal(true)}
          className="bg-emerald-500/20 text-emerald-400 hover:bg-emerald-500/30 border border-emerald-500/30 px-4 py-2 rounded-lg font-label-md transition-all shadow-[0_0_15px_rgba(16,185,129,0.1)] flex items-center gap-2"
        >
          <span className="material-symbols-outlined text-[20px]">person_add</span>
          Yeni Kullanıcı Ekle
        </button>
      </div>

      <div className="glass-panel rounded-xl flex-1 overflow-auto border-white/5 relative">
        {loading ? (
          <div className="absolute inset-0 flex items-center justify-center">
            <div className="w-8 h-8 rounded-full border-2 border-primary border-t-transparent animate-spin"></div>
          </div>
        ) : (
          <table className="w-full text-left border-collapse">
            <thead className="bg-surface-container-highest/50 sticky top-0 backdrop-blur-md z-10">
              <tr>
                <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] tracking-wider border-b border-white/5">Kullanıcı Adı</th>
                <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] tracking-wider border-b border-white/5">Email</th>
                <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] tracking-wider border-b border-white/5">Yetki</th>
                <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] tracking-wider border-b border-white/5 text-right">İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {users.map(u => (
                <tr key={u.id} className="border-b border-white/5 hover:bg-white/[0.02] transition-colors group">
                  <td className="p-4 font-body-md text-on-surface flex items-center gap-3">
                    <div className="w-8 h-8 rounded-full bg-primary/20 text-primary flex items-center justify-center font-bold">
                      {u.username.charAt(0).toUpperCase()}
                    </div>
                    {u.username}
                  </td>
                  <td className="p-4 font-body-sm text-on-surface-variant">{u.email}</td>
                  <td className="p-4">
                    {u.isAdmin ? (
                      <span className="bg-emerald-500/20 text-emerald-400 border border-emerald-500/30 px-2 py-1 rounded-md font-label-sm text-[10px] uppercase flex items-center gap-1 w-max">
                        <span className="material-symbols-outlined text-[14px]">shield</span> Admin
                      </span>
                    ) : (
                      <span className="bg-surface-container-highest text-on-surface-variant px-2 py-1 rounded-md font-label-sm text-[10px] uppercase">
                        Standart
                      </span>
                    )}
                  </td>
                  <td className="p-4 text-right">
                    <button className="text-on-surface-variant hover:text-error transition-colors p-2 rounded-full hover:bg-white/5 opacity-0 group-hover:opacity-100" title="Sil (Yakında)">
                      <span className="material-symbols-outlined text-[20px]">delete</span>
                    </button>
                  </td>
                </tr>
              ))}
              {users.length === 0 && (
                <tr>
                  <td colSpan={4} className="p-8 text-center text-on-surface-variant">Kayıtlı kullanıcı bulunamadı.</td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </div>

      {/* Add User Modal */}
      {showModal && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-background/80 backdrop-blur-sm animate-in fade-in duration-200">
          <div className="glass-panel rounded-2xl w-full max-w-md p-6 border-white/10 shadow-2xl relative overflow-hidden">
            <div className="absolute top-0 right-0 p-3 opacity-5">
              <span className="material-symbols-outlined text-[100px] text-primary">person_add</span>
            </div>
            
            <h3 className="font-display-md text-[20px] text-on-surface font-semibold mb-6">Yeni Kullanıcı Kaydı</h3>
            
            {error && <div className="bg-error-container/20 text-error p-3 rounded-lg text-sm mb-4 border border-error/30">{error}</div>}
            
            <form onSubmit={handleAddUser} className="space-y-4 relative z-10">
              <div>
                <label className="block text-on-surface-variant font-label-sm mb-1 ml-1">Kullanıcı Adı</label>
                <div className="relative">
                  <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant/50 text-[20px]">person</span>
                  <input type="text" required value={username} onChange={e => setUsername(e.target.value)}
                    className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-10 py-3 text-on-surface placeholder:text-on-surface-variant/40 focus:border-primary/50 focus:ring-1 focus:ring-primary/50 outline-none transition-all"
                    placeholder="ornek_kullanici" />
                </div>
              </div>
              
              <div>
                <label className="block text-on-surface-variant font-label-sm mb-1 ml-1">Email Adresi</label>
                <div className="relative">
                  <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant/50 text-[20px]">mail</span>
                  <input type="email" required value={email} onChange={e => setEmail(e.target.value)}
                    className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-10 py-3 text-on-surface placeholder:text-on-surface-variant/40 focus:border-primary/50 focus:ring-1 focus:ring-primary/50 outline-none transition-all"
                    placeholder="ornek@sirket.com" />
                </div>
              </div>

              <div>
                <label className="block text-on-surface-variant font-label-sm mb-1 ml-1">Geçici Şifre</label>
                <div className="relative">
                  <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant/50 text-[20px]">lock</span>
                  <input type="password" required value={password} onChange={e => setPassword(e.target.value)}
                    className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-10 py-3 text-on-surface placeholder:text-on-surface-variant/40 focus:border-primary/50 focus:ring-1 focus:ring-primary/50 outline-none transition-all"
                    placeholder="••••••••" />
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-4">
                <button type="button" onClick={() => setShowModal(false)}
                  className="px-4 py-2 text-on-surface-variant hover:text-on-surface hover:bg-white/5 rounded-lg transition-colors font-label-md">
                  İptal
                </button>
                <button type="submit" disabled={submitting}
                  className="bg-primary text-on-primary px-6 py-2 rounded-lg font-label-md shadow-[0_0_15px_rgba(68,224,146,0.3)] hover:bg-primary-fixed transition-colors active:scale-95 disabled:opacity-50 disabled:active:scale-100 flex items-center gap-2">
                  {submitting ? (
                    <span className="w-4 h-4 rounded-full border-2 border-background border-t-transparent animate-spin inline-block"></span>
                  ) : (
                    <span className="material-symbols-outlined text-[18px]">check</span>
                  )}
                  Kaydet
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
