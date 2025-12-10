import { useState } from 'react';
import { User, FileText, Calendar } from 'lucide-react';
import EmployeeProfile from './EmployeeProfile';
import EmployeeContracts from './EmployeeContracts';
import EmployeeLeaveRequests from './EmployeeLeaveRequests';

const EmployeeDetail = ({ employee, departments, positions, onEmployeeUpdate }) => {
  const [activeTab, setActiveTab] = useState('profile');

  const tabs = [
    { id: 'profile', label: 'Profile', icon: User },
    { id: 'contracts', label: 'Contracts', icon: FileText },
    { id: 'leave-requests', label: 'Leave Requests', icon: Calendar },
  ];

  if (!employee) {
    return (
      <div className="bg-white rounded-lg shadow-md border border-gray-200 h-full flex items-center justify-center">
        <div className="text-center text-gray-500">
          <User className="w-16 h-16 mx-auto mb-3 text-gray-300" />
          <p>Select an employee to view details</p>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-md border border-gray-200 h-full flex flex-col">
      {/* Header with Tabs */}
      <div className="border-b border-gray-200">
        <div className="p-4 pb-0">
          <h2 className="text-xl font-semibold text-gray-900 mb-3">
            {employee.fullName}
          </h2>
        </div>
        <div className="flex border-b border-gray-200">
          {tabs.map((tab) => {
            const Icon = tab.icon;
            return (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={`flex items-center gap-2 px-4 py-3 text-sm font-medium transition-colors border-b-2 ${
                  activeTab === tab.id
                    ? 'border-blue-600 text-blue-600'
                    : 'border-transparent text-gray-600 hover:text-gray-900 hover:border-gray-300'
                }`}
              >
                <Icon className="w-4 h-4" />
                {tab.label}
              </button>
            );
          })}
        </div>
      </div>

      {/* Tab Content */}
      <div className="flex-1 overflow-y-auto">
        {activeTab === 'profile' && (
          <EmployeeProfile
            employee={employee}
            departments={departments}
            positions={positions}
            onUpdate={onEmployeeUpdate}
          />
        )}
        {activeTab === 'contracts' && (
          <EmployeeContracts employeeId={employee.id} />
        )}
        {activeTab === 'leave-requests' && (
          <EmployeeLeaveRequests employeeId={employee.id} employeeName={employee.fullName} />
        )}
      </div>
    </div>
  );
};

export default EmployeeDetail;
