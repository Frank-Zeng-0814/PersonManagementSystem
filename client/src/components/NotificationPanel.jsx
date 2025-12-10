import { Bell, X, CheckCircle, AlertCircle, Clock, User, FileText } from 'lucide-react';

const NotificationPanel = ({ notifications, isConnected, onClear, onRemove }) => {
  const getNotificationIcon = (type) => {
    switch (type) {
      case 'EmployeeUpdated':
        return <User className="w-4 h-4" />;
      case 'LeaveRequestUpdated':
        return <FileText className="w-4 h-4" />;
      case 'ContractUpdated':
        return <FileText className="w-4 h-4" />;
      case 'ContractExpiringSoon':
        return <AlertCircle className="w-4 h-4" />;
      case 'UpcomingLeave':
        return <Clock className="w-4 h-4" />;
      default:
        return <Bell className="w-4 h-4" />;
    }
  };

  const getNotificationColor = (type) => {
    switch (type) {
      case 'ContractExpiringSoon':
        return 'bg-yellow-50 border-yellow-200 text-yellow-800';
      case 'UpcomingLeave':
        return 'bg-blue-50 border-blue-200 text-blue-800';
      case 'EmployeeUpdated':
        return 'bg-green-50 border-green-200 text-green-800';
      case 'LeaveRequestUpdated':
        return 'bg-purple-50 border-purple-200 text-purple-800';
      case 'ContractUpdated':
        return 'bg-indigo-50 border-indigo-200 text-indigo-800';
      default:
        return 'bg-gray-50 border-gray-200 text-gray-800';
    }
  };

  const formatNotificationMessage = (notification) => {
    const { type, data } = notification;
    switch (type) {
      case 'EmployeeUpdated':
        return `Employee updated: ${data.employeeName || data.employeeId}`;
      case 'LeaveRequestUpdated':
        return data.message || `Leave request status: ${data.status}`;
      case 'ContractUpdated':
        return data.message || 'Contract updated';
      case 'ContractExpiringSoon':
        return data.message || `Contract expiring for ${data.employeeName}`;
      case 'UpcomingLeave':
        return data.message || `Upcoming leave for ${data.employeeName}`;
      default:
        return JSON.stringify(data);
    }
  };

  const formatTime = (timestamp) => {
    const date = new Date(timestamp);
    const now = new Date();
    const diff = Math.floor((now - date) / 1000); // seconds

    if (diff < 60) return 'just now';
    if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
    return date.toLocaleDateString();
  };

  return (
    <div className="fixed bottom-4 right-4 w-80 max-h-96 bg-white rounded-lg shadow-lg border border-gray-200 flex flex-col overflow-hidden z-50">
      {/* Header */}
      <div className="flex items-center justify-between p-3 border-b border-gray-200 bg-gray-50">
        <div className="flex items-center gap-2">
          <Bell className="w-5 h-5 text-gray-700" />
          <h3 className="font-semibold text-gray-900">Notifications</h3>
          {notifications.length > 0 && (
            <span className="bg-blue-600 text-white text-xs rounded-full px-2 py-0.5">
              {notifications.length}
            </span>
          )}
        </div>
        <div className="flex items-center gap-2">
          {/* Connection Status */}
          <div className={`w-2 h-2 rounded-full ${isConnected ? 'bg-green-500' : 'bg-red-500'}`}
               title={isConnected ? 'Connected' : 'Disconnected'} />
          {notifications.length > 0 && (
            <button
              onClick={onClear}
              className="text-xs text-gray-600 hover:text-gray-900 underline"
            >
              Clear all
            </button>
          )}
        </div>
      </div>

      {/* Notifications List */}
      <div className="flex-1 overflow-y-auto">
        {notifications.length === 0 ? (
          <div className="p-4 text-center text-gray-500">
            <Bell className="w-8 h-8 mx-auto mb-2 text-gray-300" />
            <p className="text-sm">No notifications yet</p>
          </div>
        ) : (
          <div className="divide-y divide-gray-100">
            {notifications.map((notification) => (
              <div
                key={notification.id}
                className={`p-3 border-l-4 ${getNotificationColor(notification.type)} hover:bg-opacity-70 transition-colors`}
              >
                <div className="flex items-start justify-between gap-2">
                  <div className="flex items-start gap-2 flex-1">
                    <div className="mt-0.5">
                      {getNotificationIcon(notification.type)}
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium break-words">
                        {formatNotificationMessage(notification)}
                      </p>
                      <p className="text-xs opacity-70 mt-1">
                        {formatTime(notification.timestamp)}
                      </p>
                    </div>
                  </div>
                  <button
                    onClick={() => onRemove(notification.id)}
                    className="p-1 hover:bg-white hover:bg-opacity-50 rounded"
                  >
                    <X className="w-3 h-3" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default NotificationPanel;
