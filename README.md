# SmartAppointmentScheduler.API
# Description
	This project is a RESTful API that enables secure appointment booking with role-based access. 
It supports user authentication, appointment scheduling, admin approvals, and conflict prevention.

# Technology Stack
 ASP.NET Core Web API, EF Core, SQL Server, JWT Authentication

# Security
    * JWT Authentication
    * Role-based authorization
    * Secure password hashing
    * Protected admin endpoints

# API Endpoints
Authentication
---------------
POST /api/auth/register
POST /api/auth/login

Admin (Admin only)
------
GET /api/admin/GetAppointment\
POST /api/admin/CreateSlot\
GET /api/admin/GetAppointment\
POST /api/admin/UpdatePending\
POST /api/admin/MarkCompleted\

Appointments
-------------
POST /api/appointments/book
GET /api/appointments/GetAppointments
POSt /api/appointments/GetTimeSlot
POSt /api/appointments/CancelleAppointment (User only)
