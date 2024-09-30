# Equipment Rental Management System

## Overview

This system is designed to manage the lending of technical tools and equipment across multiple studios. Each studio owns specific equipment, and each piece of equipment has unique attributes such as the year of manufacture, purchase date, and an image. The owner of the equipment sets the maximum rental duration for each item. Information regarding where and when equipment can be picked up must also be provided.

The system organizes equipment into categories (e.g., computer equipment, cameras, microphones, etc.). These categories are shared among all studios, but only the studio that owns the equipment can manage it. Both students and instructors assigned to a specific studio can reserve and borrow equipment. Additionally, the studio manager can restrict the borrowing of certain equipment to specific users within their studio.

## User Roles and Functionality

The system includes several roles, each with specific rights and responsibilities:

### Administrator
- **Manages users:** Can create and manage user accounts.
- **Creates and manages studios:** Responsible for setting up and maintaining studio information.
- **Assigns studio manager rights:** Can grant studio manager permissions to a registered user.
- **Has access to all other roles' rights:** Inherits the abilities of all other roles below.

### Studio Manager
- **Manages equipment categories:** Can create and manage equipment types for their studio.
- **Assigns instructor rights:** Can assign registered users as instructors for their studio.
- **Manages users in their studio:** Can assign users to their studio.
- **Has the rights of a registered user:** Can borrow and reserve equipment, view available equipment, and more.

### Instructor
- **Manages equipment:** Can add and manage equipment for their studio.
- **Can temporarily block equipment:** Has the ability to prevent certain equipment from being borrowed.
- **Manages user access:** Can update the list of registered users assigned to the studio, determining who can borrow equipment.
- **Has the rights of a registered user:** Can reserve and borrow equipment, track rentals, and more.

### Registered User
- **Views available equipment:** Can browse the equipment that is available for them to borrow.
- **Borrows equipment:** Can check out equipment for a specified duration.
- **Reserves equipment:** Can reserve equipment in advance.
- **Tracks rental status:** Can view the current status of their equipment rentals.
- **Edits profile:** Can update their personal information within the system.
- **Has the rights of an unregistered user:** Can access general system features like registration.

### Unregistered User
- **Registration access:** Can sign up to become a registered user within the system.

## System Features

The system supports the following actions based on the user roles:

- **Equipment Grouping:** Equipment is grouped by type, with shared categories across studios.
- **Role-Based Permissions:** Access to equipment management and borrowing is controlled by user roles (admin, studio manager, instructor, registered user).
- **User-Specific Restrictions:** Studio managers can limit the borrowing of certain equipment to specific users within their studio.

