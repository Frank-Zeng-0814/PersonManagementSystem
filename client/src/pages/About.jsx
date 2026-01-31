import { Code, Database, Cloud, Zap, Globe, Sparkles, Bell } from 'lucide-react';

const About = () => {
    const apiUrl = import.meta.env.VITE_BASE_API_URL || 'http://localhost:8080';
    const swaggerUrl = `${apiUrl}/swagger`;

    const apiEndpoints = [
        { method: "GET", path: "/api/employees", description: "Get paginated employees with search & filters" },
        { method: "POST", path: "/api/employees", description: "Create new employee" },
        { method: "PUT", path: "/api/employees/{id}", description: "Update employee" },
        { method: "POST", path: "/api/employees/{id}/upload-avatar", description: "Upload employee avatar" },
        { method: "GET", path: "/api/employees/{id}/contracts", description: "Get contracts for an employee" },
        { method: "POST", path: "/api/employees/{id}/contracts", description: "Create employment contract" },
        { method: "GET", path: "/api/employees/{id}/leave-requests", description: "Get leave requests for an employee" },
        { method: "POST", path: "/api/leave-requests/{id}/submit", description: "Submit leave request for approval" },
        { method: "POST", path: "/api/leave-requests/{id}/approve", description: "Approve a leave request" },
        { method: "POST", path: "/api/leave-requests/{id}/reject", description: "Reject a leave request" }
    ];

    const architecture = [
        {
            icon: <Code className="w-6 h-6" />,
            title: "Clean Architecture",
            description: "Separation of concerns with DTOs, Services, and Controllers"
        },
        {
            icon: <Database className="w-6 h-6" />,
            title: "Entity Framework Core",
            description: "Code-first approach with auto migrations and PostgreSQL"
        },
        {
            icon: <Bell className="w-6 h-6" />,
            title: "Real-time Notifications",
            description: "SignalR integration for instant push notifications"
        },
        {
            icon: <Cloud className="w-6 h-6" />,
            title: "Cloud Storage",
            description: "Cloudinary integration for employee avatar management"
        },
        {
            icon: <Zap className="w-6 h-6" />,
            title: "Leave Approval Workflow",
            description: "State machine-based workflow: Draft → Submitted → Approved / Rejected"
        }
    ];

    return (
        <div className="min-h-screen bg-gray-50 py-12">
            <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">

                <div className="text-center mb-12">
                    <h1 className="text-4xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent mb-4">
                        About This Project
                    </h1>
                    <p className="text-lg text-gray-600 max-w-2xl mx-auto">
                        A production-ready HR management system built with .NET 8 and React 19
                    </p>
                </div>

                <div className="grid md:grid-cols-2 gap-6 mb-12">
                    {architecture.map((item, index) => (
                        <div key={index} className="bg-white rounded-lg p-6 shadow-md border border-gray-200">
                            <div className="flex items-start space-x-4">
                                <div className="text-blue-500 mt-1">{item.icon}</div>
                                <div>
                                    <h3 className="font-semibold text-gray-900 mb-1">{item.title}</h3>
                                    <p className="text-gray-600 text-sm">{item.description}</p>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                <div className="bg-white rounded-xl shadow-lg p-8 border border-gray-200 mb-12">
                    <div className="flex items-center mb-6">
                        <Globe className="w-6 h-6 text-blue-500 mr-3" />
                        <h2 className="text-2xl font-bold text-gray-900">API Endpoints</h2>
                    </div>
                    <div className="space-y-3">
                        {apiEndpoints.map((endpoint, index) => (
                            <div key={index} className="flex items-start space-x-4 p-4 bg-gray-50 rounded-lg">
                                <span className={`px-3 py-1 rounded text-xs font-semibold ${
                                    endpoint.method === 'GET' ? 'bg-green-100 text-green-700' :
                                    endpoint.method === 'POST' ? 'bg-blue-100 text-blue-700' :
                                    endpoint.method === 'PUT' ? 'bg-yellow-100 text-yellow-700' :
                                    'bg-red-100 text-red-700'
                                }`}>
                                    {endpoint.method}
                                </span>
                                <div className="flex-1">
                                    <code className="text-sm font-mono text-gray-800">{endpoint.path}</code>
                                    <p className="text-sm text-gray-600 mt-1">{endpoint.description}</p>
                                </div>
                            </div>
                        ))}
                    </div>
                    <div className="mt-6 p-4 bg-blue-50 rounded-lg">
                        <p className="text-sm text-blue-800">
                            <strong>Swagger Documentation:</strong> Access interactive API docs at{' '}
                            <a href={swaggerUrl} target="_blank" rel="noopener noreferrer" className="underline hover:text-blue-600">
                                {swaggerUrl}
                            </a>
                        </p>
                        <p className="text-xs text-blue-600 mt-2">
                            💡 Note: Swagger is only available when the backend API is running
                        </p>
                    </div>
                </div>

                <div className="bg-gradient-to-r from-blue-500 to-purple-500 rounded-xl shadow-lg p-8 text-white text-center">
                    <Sparkles className="w-12 h-12 mx-auto mb-4" />
                    <h2 className="text-2xl font-bold mb-2">Modern Development</h2>
                    <p className="mb-6 opacity-90">
                        Built with modern technologies and best practices
                    </p>
                    <div className="flex flex-wrap justify-center gap-3 text-sm">
                        <span className="px-4 py-2 bg-white/20 rounded-full">Clean Architecture</span>
                        <span className="px-4 py-2 bg-white/20 rounded-full">Real-time Notifications</span>
                        <span className="px-4 py-2 bg-white/20 rounded-full">Workflow Management</span>
                        <span className="px-4 py-2 bg-white/20 rounded-full">Structured Logging</span>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default About;