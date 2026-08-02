import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';

interface RoleDto {
  id: string;
  name: string;
}

interface PermissionDto {
  id: string;
  systemName: string;
  description: string;
}

interface UserDto {
  id: string;
  username: string;
  email: string;
  isAdmin: boolean;
  roles: RoleDto[];
}

export const UserManagement: React.FC = () => {
  const { t } = useTranslation();
  const { token } = useAuth();
  
  const [activeTab, setActiveTab] = useState<'users' | 'roles' | 'permissions'>('users');
  
  const [users, setUsers] = useState<UserDto[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [permissions, setPermissions] = useState<PermissionDto[]>([]);
  
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Users Tab Modals
  const [showAddUserModal, setShowAddUserModal] = useState(false);
  const [showAssignRoleModal, setShowAssignRoleModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);

  // Roles Tab Modals
  const [showAddRoleModal, setShowAddRoleModal] = useState(false);
  const [showAssignPermissionModal, setShowAssignPermissionModal] = useState(false);
  const [selectedRole, setSelectedRole] = useState<RoleDto | null>(null);

  // Form States
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [roleName, setRoleName] = useState('');
  const [selectedRoleIdToAssign, setSelectedRoleIdToAssign] = useState('');
  const [selectedPermissionIdToAssign, setSelectedPermissionIdToAssign] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const extractErrorCode = (errData: any): string => {
    if (errData.error) return errData.error;
    if (errData.errors) {
      const firstKey = Object.keys(errData.errors)[0];
      if (firstKey && errData.errors[firstKey].length > 0) {
        return errData.errors[firstKey][0];
      }
    }
    return 'GENERAL_ERROR';
  };

  const fetchUsers = async () => {
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/users`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) setUsers(await res.json());
    } catch (err) {
      console.error(err);
    }
  };

  const fetchRoles = async () => {
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/roles`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) setRoles(await res.json());
    } catch (err) {
      console.error(err);
    }
  };

  const fetchPermissions = async () => {
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/permissions`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (res.ok) setPermissions(await res.json());
    } catch (err) {
      console.error(err);
    }
  };

  const loadData = async () => {
    setLoading(true);
    setError(null);
    await Promise.all([fetchUsers(), fetchRoles(), fetchPermissions()]);
    setLoading(false);
  };

  useEffect(() => {
    if (token) loadData();
  }, [token]);

  // Handlers
  const handleAddUser = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({ username, email, password })
      });
      if (res.ok) {
        setShowAddUserModal(false);
        setUsername(''); setEmail(''); setPassword('');
        await fetchUsers();
      } else {
        const errData = await res.json();
        setError(t(`errors.${extractErrorCode(errData)}`, { defaultValue: t('errors.GENERAL_ERROR') }));
      }
    } catch {
      setError(t('errors.CONNECTION_ERROR'));
    } finally {
      setSubmitting(false);
    }
  };

  const handleAddRole = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/roles`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({ name: roleName })
      });
      if (res.ok) {
        setShowAddRoleModal(false);
        setRoleName('');
        await fetchRoles();
      } else {
        const errData = await res.json();
        setError(t(`errors.${extractErrorCode(errData)}`, { defaultValue: t('errors.GENERAL_ERROR') }));
      }
    } catch {
      setError(t('errors.CONNECTION_ERROR'));
    } finally {
      setSubmitting(false);
    }
  };

  const handleAssignRole = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedUser || !selectedRoleIdToAssign) return;
    setSubmitting(true);
    setError(null);
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/users/${selectedUser.id}/roles`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({ roleId: selectedRoleIdToAssign })
      });
      if (res.ok) {
        setShowAssignRoleModal(false);
        setSelectedRoleIdToAssign('');
        await fetchUsers(); // refresh roles of user
      } else {
        const errData = await res.json();
        setError(t(`errors.${extractErrorCode(errData)}`, { defaultValue: t('errors.GENERAL_ERROR') }));
      }
    } catch {
      setError(t('errors.CONNECTION_ERROR'));
    } finally {
      setSubmitting(false);
    }
  };

  const handleAssignPermission = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedRole || !selectedPermissionIdToAssign) return;
    setSubmitting(true);
    setError(null);
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/auth/roles/${selectedRole.id}/permissions`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({ permissionId: selectedPermissionIdToAssign })
      });
      if (res.ok) {
        setShowAssignPermissionModal(false);
        setSelectedPermissionIdToAssign('');
        // Permission list in roles isn't currently tracked in UI state, but API call succeeded.
      } else {
        const errData = await res.json();
        setError(t(`errors.${extractErrorCode(errData)}`, { defaultValue: t('errors.GENERAL_ERROR') }));
      }
    } catch {
      setError(t('errors.CONNECTION_ERROR'));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="flex-1 flex flex-col p-gutter h-full overflow-hidden animate-in fade-in duration-300">
      <div className="flex justify-between items-start mb-md">
        <div>
          <h2 className="font-display-md text-on-surface text-[24px] font-bold">Kullanıcı & Rol Yönetimi</h2>
          <p className="text-on-surface-variant font-body-sm mt-1">Sisteme kayıtlı kullanıcıları, rolleri ve yetkileri buradan yönetebilirsiniz.</p>
        </div>
        <div className="flex gap-2">
          {activeTab === 'users' && (
            <button onClick={() => setShowAddUserModal(true)} className="bg-emerald-500/20 text-emerald-400 hover:bg-emerald-500/30 border border-emerald-500/30 px-4 py-2 rounded-lg font-label-md transition-all flex items-center gap-2">
              <span className="material-symbols-outlined text-[18px]">person_add</span> Yeni Kullanıcı
            </button>
          )}
          {activeTab === 'roles' && (
            <button onClick={() => setShowAddRoleModal(true)} className="bg-primary/20 text-primary hover:bg-primary/30 border border-primary/30 px-4 py-2 rounded-lg font-label-md transition-all flex items-center gap-2">
              <span className="material-symbols-outlined text-[18px]">add_moderator</span> Yeni Rol
            </button>
          )}
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-2 mb-4 p-1 bg-surface-container-highest/30 rounded-xl w-max">
        <button onClick={() => setActiveTab('users')} className={`px-4 py-2 rounded-lg font-label-md transition-all flex items-center gap-2 ${activeTab === 'users' ? 'bg-surface-container-highest text-on-surface shadow-sm' : 'text-on-surface-variant hover:text-on-surface'}`}>
          <span className="material-symbols-outlined text-[18px]">group</span> Kullanıcılar
        </button>
        <button onClick={() => setActiveTab('roles')} className={`px-4 py-2 rounded-lg font-label-md transition-all flex items-center gap-2 ${activeTab === 'roles' ? 'bg-surface-container-highest text-on-surface shadow-sm' : 'text-on-surface-variant hover:text-on-surface'}`}>
          <span className="material-symbols-outlined text-[18px]">admin_panel_settings</span> Roller
        </button>
        <button onClick={() => setActiveTab('permissions')} className={`px-4 py-2 rounded-lg font-label-md transition-all flex items-center gap-2 ${activeTab === 'permissions' ? 'bg-surface-container-highest text-on-surface shadow-sm' : 'text-on-surface-variant hover:text-on-surface'}`}>
          <span className="material-symbols-outlined text-[18px]">key</span> Yetkiler
        </button>
      </div>

      <div className="glass-panel rounded-xl flex-1 overflow-auto border-white/5 relative">
        {loading ? (
          <div className="absolute inset-0 flex items-center justify-center">
            <div className="w-8 h-8 rounded-full border-2 border-primary border-t-transparent animate-spin"></div>
          </div>
        ) : (
          <div className="animate-in fade-in">
            {/* USERS TAB */}
            {activeTab === 'users' && (
              <table className="w-full text-left border-collapse">
                <thead className="bg-surface-container-highest/50 sticky top-0 backdrop-blur-md z-10">
                  <tr>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5">Kullanıcı</th>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5">Email</th>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5">Roller</th>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5 text-right">İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map(u => (
                    <tr key={u.id} className="border-b border-white/5 hover:bg-white/[0.02] group transition-colors">
                      <td className="p-4 font-body-md text-on-surface flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-primary/20 text-primary flex items-center justify-center font-bold">
                          {u.username.charAt(0).toUpperCase()}
                        </div>
                        {u.username}
                      </td>
                      <td className="p-4 font-body-sm text-on-surface-variant">{u.email}</td>
                      <td className="p-4">
                        <div className="flex flex-wrap gap-1">
                          {u.roles && u.roles.length > 0 ? (
                            u.roles.map(r => (
                              <span key={r.id} className="bg-surface-container-highest text-primary-fixed border border-primary/20 px-2 py-1 rounded-md font-label-sm text-[10px] uppercase">
                                {r.name}
                              </span>
                            ))
                          ) : (
                            <span className="text-on-surface-variant/50 text-xs italic">Rol Yok</span>
                          )}
                        </div>
                      </td>
                      <td className="p-4 text-right">
                        <button onClick={() => { setSelectedUser(u); setShowAssignRoleModal(true); setError(null); }} className="text-primary hover:text-primary-fixed transition-colors p-2 rounded-full hover:bg-primary/10 opacity-0 group-hover:opacity-100" title="Rol Ata">
                          <span className="material-symbols-outlined text-[20px]">badge</span>
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}

            {/* ROLES TAB */}
            {activeTab === 'roles' && (
              <table className="w-full text-left border-collapse">
                <thead className="bg-surface-container-highest/50 sticky top-0 backdrop-blur-md z-10">
                  <tr>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5 w-1/3">Rol Adı</th>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5 w-1/3">ID</th>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5 text-right">İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {roles.map(r => (
                    <tr key={r.id} className="border-b border-white/5 hover:bg-white/[0.02] group transition-colors">
                      <td className="p-4 font-body-md text-on-surface font-semibold text-primary">{r.name}</td>
                      <td className="p-4 font-mono text-[11px] text-on-surface-variant/50">{r.id}</td>
                      <td className="p-4 text-right">
                        <button onClick={() => { setSelectedRole(r); setShowAssignPermissionModal(true); setError(null); }} className="text-emerald-400 hover:text-emerald-300 transition-colors p-2 rounded-full hover:bg-emerald-400/10 opacity-0 group-hover:opacity-100" title="Yetki Ata">
                          <span className="material-symbols-outlined text-[20px]">vpn_key</span>
                        </button>
                      </td>
                    </tr>
                  ))}
                  {roles.length === 0 && <tr><td colSpan={3} className="p-8 text-center text-on-surface-variant">Kayıtlı rol bulunamadı.</td></tr>}
                </tbody>
              </table>
            )}

            {/* PERMISSIONS TAB */}
            {activeTab === 'permissions' && (
              <table className="w-full text-left border-collapse">
                <thead className="bg-surface-container-highest/50 sticky top-0 backdrop-blur-md z-10">
                  <tr>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5">Sistem Adı</th>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5">Açıklama</th>
                    <th className="p-4 font-label-md text-on-surface-variant uppercase text-[12px] border-b border-white/5">ID</th>
                  </tr>
                </thead>
                <tbody>
                  {permissions.map(p => (
                    <tr key={p.id} className="border-b border-white/5 hover:bg-white/[0.02] transition-colors">
                      <td className="p-4 font-mono text-[12px] text-primary">{p.systemName}</td>
                      <td className="p-4 font-body-sm text-on-surface-variant">{p.description}</td>
                      <td className="p-4 font-mono text-[11px] text-on-surface-variant/50">{p.id}</td>
                    </tr>
                  ))}
                  {permissions.length === 0 && <tr><td colSpan={3} className="p-8 text-center text-on-surface-variant">Sistemde yetki tanımı bulunmamaktadır. Veritabanından (Seed) eklenmelidir.</td></tr>}
                </tbody>
              </table>
            )}
          </div>
        )}
      </div>

      {/* Add User Modal */}
      {showAddUserModal && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-background/80 backdrop-blur-sm animate-in fade-in">
          <div className="glass-panel rounded-2xl w-full max-w-md p-6 border-white/10 shadow-2xl">
            <h3 className="font-display-md text-[20px] text-on-surface font-semibold mb-6">Yeni Kullanıcı Kaydı</h3>
            {error && <div className="bg-error-container/20 text-error p-3 rounded-lg text-sm mb-4">{error}</div>}
            <form onSubmit={handleAddUser} className="space-y-4">
              <input type="text" required value={username} onChange={e => setUsername(e.target.value)} placeholder="Kullanıcı Adı" className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-4 py-3 text-on-surface outline-none" />
              <input type="email" required value={email} onChange={e => setEmail(e.target.value)} placeholder="Email" className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-4 py-3 text-on-surface outline-none" />
              <input type="password" required value={password} onChange={e => setPassword(e.target.value)} placeholder="Geçici Şifre" className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-4 py-3 text-on-surface outline-none" />
              <div className="flex justify-end gap-3 pt-4">
                <button type="button" onClick={() => setShowAddUserModal(false)} className="px-4 py-2 text-on-surface-variant hover:text-on-surface">İptal</button>
                <button type="submit" disabled={submitting} className="bg-primary text-background px-6 py-2 rounded-lg">Kaydet</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Add Role Modal */}
      {showAddRoleModal && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-background/80 backdrop-blur-sm animate-in fade-in">
          <div className="glass-panel rounded-2xl w-full max-w-md p-6 border-white/10 shadow-2xl">
            <h3 className="font-display-md text-[20px] text-on-surface font-semibold mb-6">Yeni Rol Ekle</h3>
            {error && <div className="bg-error-container/20 text-error p-3 rounded-lg text-sm mb-4">{error}</div>}
            <form onSubmit={handleAddRole} className="space-y-4">
              <input type="text" required value={roleName} onChange={e => setRoleName(e.target.value)} placeholder="Rol Adı (örn: Editor)" className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-4 py-3 text-on-surface outline-none" />
              <div className="flex justify-end gap-3 pt-4">
                <button type="button" onClick={() => setShowAddRoleModal(false)} className="px-4 py-2 text-on-surface-variant hover:text-on-surface">İptal</button>
                <button type="submit" disabled={submitting} className="bg-primary text-background px-6 py-2 rounded-lg">Kaydet</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Assign Role Modal */}
      {showAssignRoleModal && selectedUser && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-background/80 backdrop-blur-sm animate-in fade-in">
          <div className="glass-panel rounded-2xl w-full max-w-md p-6 border-white/10 shadow-2xl">
            <h3 className="font-display-md text-[20px] text-on-surface font-semibold mb-2">Rol Ata: {selectedUser.username}</h3>
            <p className="text-sm text-on-surface-variant mb-6">Kullanıcıya atanacak rolü seçin.</p>
            {error && <div className="bg-error-container/20 text-error p-3 rounded-lg text-sm mb-4">{error}</div>}
            <form onSubmit={handleAssignRole} className="space-y-4">
              <select required value={selectedRoleIdToAssign} onChange={e => setSelectedRoleIdToAssign(e.target.value)} className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-4 py-3 text-on-surface outline-none appearance-none">
                <option value="" disabled>Bir rol seçin...</option>
                {roles.map(r => (
                  <option key={r.id} value={r.id}>{r.name}</option>
                ))}
              </select>
              <div className="flex justify-end gap-3 pt-4">
                <button type="button" onClick={() => setShowAssignRoleModal(false)} className="px-4 py-2 text-on-surface-variant hover:text-on-surface">İptal</button>
                <button type="submit" disabled={submitting} className="bg-primary text-background px-6 py-2 rounded-lg">Ata</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Assign Permission Modal */}
      {showAssignPermissionModal && selectedRole && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-background/80 backdrop-blur-sm animate-in fade-in">
          <div className="glass-panel rounded-2xl w-full max-w-md p-6 border-white/10 shadow-2xl">
            <h3 className="font-display-md text-[20px] text-on-surface font-semibold mb-2">Yetki Ata: {selectedRole.name}</h3>
            <p className="text-sm text-on-surface-variant mb-6">Role atanacak yetkiyi seçin.</p>
            {error && <div className="bg-error-container/20 text-error p-3 rounded-lg text-sm mb-4">{error}</div>}
            <form onSubmit={handleAssignPermission} className="space-y-4">
              <select required value={selectedPermissionIdToAssign} onChange={e => setSelectedPermissionIdToAssign(e.target.value)} className="w-full bg-surface-container-highest border border-white/10 rounded-xl px-4 py-3 text-on-surface outline-none appearance-none">
                <option value="" disabled>Bir yetki seçin...</option>
                {permissions.map(p => (
                  <option key={p.id} value={p.id}>{p.systemName}</option>
                ))}
              </select>
              <div className="flex justify-end gap-3 pt-4">
                <button type="button" onClick={() => setShowAssignPermissionModal(false)} className="px-4 py-2 text-on-surface-variant hover:text-on-surface">İptal</button>
                <button type="submit" disabled={submitting} className="bg-primary text-background px-6 py-2 rounded-lg">Ata</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
