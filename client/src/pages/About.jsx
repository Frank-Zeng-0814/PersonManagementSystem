import { Code, Database, Cloud, Zap, Globe, Sparkles } from 'lucide-react';

const About = () => {
    const apiEndpoints = [
        { method: "GET", path: "/api/people", description: "Get paginated people with search & filters" },
        { method: "GET", path: "/api/people/{id}", description: "Get person by ID" },
        { method: "POST", path: "/api/people", description: "Create new person" },
        { method: "PUT", path: "/api/people/{id}", description: "Update person" },
        { method: "DELETE", path: "/api/people/{id}", description: "Delete person" },
        { method: "POST", path: "/api/people/{id}/upload-avatar", description: "Upload person avatar" }
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
            description: "Code-first approach with auto migrations"
        },
        {
            icon: <Cloud className="w-6 h-6" />,
            title: "Cloud Storage",
            description: "Cloudinary integration for image management"
        },
        {
            icon: <Zap className="w-6 h-6" />,
            title: "Performance",
            description: "LINQ queries with pagination and filtering"
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
                        A production-ready full-stack application demonstrating modern web development best practices
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
                            <a href="http://localhost:3000/swagger" target="_blank" rel="noopener noreferrer" className="underline">
                                http://localhost:3000/swagger
                            </a>
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
                        <span className="px-4 py-2 bg-white/20 rounded-full">Clean Code</span>
                        <span className="px-4 py-2 bg-white/20 rounded-full">Error Handling</span>
                        <span className="px-4 py-2 bg-white/20 rounded-full">Logging</span>
                        <span className="px-4 py-2 bg-white/20 rounded-full">Security</span>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default About;