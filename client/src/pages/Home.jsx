import { Database, Search, Upload, Shield, FileText, ArrowRight } from 'lucide-react';
import { Link } from 'react-router-dom';

const Home = () => {
    const features = [
        {
            icon: <Search className="w-8 h-8" />,
            title: "Advanced Search & Filtering",
            description: "Search by name with real-time filtering and multi-field sorting capabilities"
        },
        {
            icon: <Database className="w-8 h-8" />,
            title: "Smart Pagination",
            description: "Efficient data handling with customizable page sizes and navigation"
        },
        {
            icon: <Upload className="w-8 h-8" />,
            title: "Cloud Image Upload",
            description: "Secure avatar uploads with Cloudinary integration and validation"
        },
        {
            icon: <Shield className="w-8 h-8" />,
            title: "Error Handling & Logging",
            description: "Production-grade error handling with Serilog structured logging"
        },
        {
            icon: <FileText className="w-8 h-8" />,
            title: "API Documentation",
            description: "Interactive Swagger documentation for all endpoints"
        }
    ];

    const techStack = {
        backend: [".NET 8", "Entity Framework Core", "PostgreSQL", "Serilog", "Cloudinary"],
        frontend: ["React 19", "Tailwind CSS", "React Router", "React Hook Form", "Axios"]
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 via-purple-50 to-pink-50">
            <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-16">

                <div className="text-center mb-16">
                    <h1 className="text-5xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent mb-4">
                        Person Management System
                    </h1>
                    <p className="text-xl text-gray-600 mb-8">
                        A full-stack CRUD application showcasing modern web development practices
                    </p>
                    <Link
                        to="/person"
                        className="inline-flex items-center px-8 py-4 bg-gradient-to-r from-blue-500 to-purple-500 text-white font-semibold rounded-lg hover:from-blue-600 hover:to-purple-600 transition-all duration-200 transform hover:scale-105 shadow-lg"
                    >
                        Get Started
                        <ArrowRight className="ml-2 w-5 h-5" />
                    </Link>
                </div>

                <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6 mb-16">
                    {features.map((feature, index) => (
                        <div
                            key={index}
                            className="bg-white rounded-xl p-6 shadow-lg border border-gray-100 hover:shadow-xl transition-shadow duration-200"
                        >
                            <div className="text-blue-500 mb-4">
                                {feature.icon}
                            </div>
                            <h3 className="text-lg font-semibold text-gray-900 mb-2">
                                {feature.title}
                            </h3>
                            <p className="text-gray-600 text-sm">
                                {feature.description}
                            </p>
                        </div>
                    ))}
                </div>

                <div className="bg-white rounded-xl shadow-lg p-8 border border-gray-100">
                    <h2 className="text-2xl font-bold text-gray-900 mb-6 text-center">
                        Technology Stack
                    </h2>
                    <div className="grid md:grid-cols-2 gap-8">
                        <div>
                            <h3 className="text-lg font-semibold text-blue-600 mb-4">Backend</h3>
                            <div className="flex flex-wrap gap-2">
                                {techStack.backend.map((tech, index) => (
                                    <span
                                        key={index}
                                        className="px-4 py-2 bg-blue-100 text-blue-700 rounded-full text-sm font-medium"
                                    >
                                        {tech}
                                    </span>
                                ))}
                            </div>
                        </div>
                        <div>
                            <h3 className="text-lg font-semibold text-purple-600 mb-4">Frontend</h3>
                            <div className="flex flex-wrap gap-2">
                                {techStack.frontend.map((tech, index) => (
                                    <span
                                        key={index}
                                        className="px-4 py-2 bg-purple-100 text-purple-700 rounded-full text-sm font-medium"
                                    >
                                        {tech}
                                    </span>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Home;