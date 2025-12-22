import { useEffect, useState, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

const useNotifications = () => {
  const [notifications, setNotifications] = useState([]);
  const [isConnected, setIsConnected] = useState(false);
  const connectionRef = useRef(null);

  const addNotification = useCallback((type, data) => {
    const notification = {
      id: Date.now(),
      type,
      data,
      timestamp: new Date().toISOString(),
    };
    setNotifications(prev => [notification, ...prev].slice(0, 50)); // Keep last 50 notifications
  }, []);

  const clearNotifications = useCallback(() => {
    setNotifications([]);
  }, []);

  const removeNotification = useCallback((id) => {
    setNotifications(prev => prev.filter(n => n.id !== id));
  }, []);

  useEffect(() => {
    const baseUrl = import.meta.env.VITE_BASE_API_URL || 'http://localhost:5000';
    const hubUrl = `${baseUrl}/hubs/notifications`;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connectionRef.current = connection;

    // Event handlers
    connection.on('EmployeeUpdated', (data) => {
      console.log('EmployeeUpdated:', data);
      addNotification('EmployeeUpdated', data);
    });

    connection.on('LeaveRequestUpdated', (data) => {
      console.log('LeaveRequestUpdated:', data);
      addNotification('LeaveRequestUpdated', data);
    });

    connection.on('ContractUpdated', (data) => {
      console.log('ContractUpdated:', data);
      addNotification('ContractUpdated', data);
    });

    connection.on('ContractExpiringSoon', (data) => {
      console.log('ContractExpiringSoon:', data);
      addNotification('ContractExpiringSoon', data);
    });

    connection.on('UpcomingLeave', (data) => {
      console.log('UpcomingLeave:', data);
      addNotification('UpcomingLeave', data);
    });

    // Start connection
    const startConnection = async () => {
      try {
        await connection.start();
        console.log('SignalR Connected');
        setIsConnected(true);
      } catch (err) {
        console.error('SignalR Connection Error:', err);
        setIsConnected(false);
        // Retry connection after 5 seconds
        setTimeout(startConnection, 5000);
      }
    };

    startConnection();

    // Handle reconnection events
    connection.onreconnecting(() => {
      console.log('SignalR Reconnecting...');
      setIsConnected(false);
    });

    connection.onreconnected(() => {
      console.log('SignalR Reconnected');
      setIsConnected(true);
    });

    connection.onclose(() => {
      console.log('SignalR Connection Closed');
      setIsConnected(false);
    });

    // Cleanup
    return () => {
      if (connection) {
        connection.stop();
      }
    };
  }, [addNotification]);

  return {
    notifications,
    isConnected,
    clearNotifications,
    removeNotification,
  };
};

export default useNotifications;
