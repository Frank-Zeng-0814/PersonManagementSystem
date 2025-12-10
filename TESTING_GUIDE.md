# 🧪 Employee Management System - Testing Guide

## 📋 Prerequisites

### Backend Setup
1. Ensure PostgreSQL is running
2. Configure `.env` file in `Backend/Backend/`:
   ```
   ConnectionStrings__PostgreSQL=your_connection_string
   Cloudinary__CloudName=your_cloudinary_name
   Cloudinary__ApiKey=your_cloudinary_key
   Cloudinary__ApiSecret=your_cloudinary_secret
   ```
3. Start backend:
   ```bash
   cd Backend/Backend
   dotnet run
   ```
   Backend should be running on `http://localhost:3000`

### Frontend Setup
1. Configure `.env` file in `client/`:
   ```
   VITE_BASE_API_URL=http://localhost:3000
   ```
2. Start frontend:
   ```bash
   cd client
   npm run dev
   ```
   Frontend should be running on `http://localhost:5173`

---

## 🎯 Complete Testing Checklist

### 1. Department Management APIs

#### ✅ Test: GET /api/departments
**Expected Result:** Returns list of all departments

**Steps:**
1. Open browser DevTools → Network tab
2. Navigate to Person page
3. Check for `/api/departments` request
4. Should return status `200` with array of departments

#### ✅ Test: POST /api/departments
**Expected Result:** Create new department

**Manual Test via Swagger/Postman:**
```json
POST http://localhost:3000/api/departments
Content-Type: application/json

{
  "name": "Engineering",
  "description": "Software Development Team"
}
```
**Expected:** Status `201`, returns created department with `id`

#### ✅ Test: PUT /api/departments/{id}
**Expected Result:** Update department

```json
PUT http://localhost:3000/api/departments/1
Content-Type: application/json

{
  "name": "Engineering - Updated",
  "description": "Updated description"
}
```
**Expected:** Status `200`, returns updated department

#### ✅ Test: DELETE /api/departments/{id}
**Expected Result:** Delete department

```
DELETE http://localhost:3000/api/departments/1
```
**Expected:** Status `204` (No Content)

---

### 2. Position Management APIs

#### ✅ Test: GET /api/positions
**Steps:**
1. Navigate to Person page
2. Check Network tab for `/api/positions` request
3. Should return status `200` with array of positions

#### ✅ Test: POST /api/positions
```json
POST http://localhost:3000/api/positions
Content-Type: application/json

{
  "title": "Senior Software Engineer",
  "description": "Lead development projects"
}
```
**Expected:** Status `201`

#### ✅ Test: PUT /api/positions/{id}
```json
PUT http://localhost:3000/api/positions/1
Content-Type: application/json

{
  "title": "Staff Software Engineer",
  "description": "Updated role"
}
```
**Expected:** Status `200`

#### ✅ Test: DELETE /api/positions/{id}
```
DELETE http://localhost:3000/api/positions/1
```
**Expected:** Status `204`

---

### 3. Employee Management (CRUD)

#### ✅ Test: GET /api/employees
**UI Steps:**
1. Navigate to `/person` page
2. Should see employee list on the left
3. Check Network tab → `/api/employees` returns `200`

**Expected UI:**
- Employee list with avatars
- Employee names, emails
- Department & Position displayed
- Status badges (Active/OnLeave/Inactive)

#### ✅ Test: POST /api/employees (Create Employee)
**UI Steps:**
1. Click "New" button in employee list
2. Modal should appear with white background (not black!)
3. Fill in form:
   - Full Name: "John Doe"
   - Email: "john@example.com"
   - Phone: "+1 234 567 8900"
   - Department: Select one
   - Position: Select one
4. Click "Create Employee"

**Expected:**
- Success toast notification
- Modal closes
- New employee appears in list
- Network shows `201` status

#### ✅ Test: PUT /api/employees/{id} (Update Employee)
**UI Steps:**
1. Click "Edit" button (pencil icon) on any employee
2. Modal appears with employee data pre-filled
3. Modify any field (e.g., change name to "John Smith")
4. Click "Update Employee"

**Expected:**
- Success toast notification
- Modal closes
- Employee list updated with new data
- Network shows `200` status

#### ✅ Test: DELETE /api/employees/{id}
**UI Steps:**
1. Click "Delete" button (trash icon) on any employee
2. Confirmation dialog appears
3. Click "OK"

**Expected:**
- Success toast notification
- Employee removed from list
- Network shows `204` status

#### ✅ Test: Upload Avatar
**UI Steps:**
1. Hover over employee avatar in the list
2. Upload icon appears as overlay
3. Click to select image file (JPEG/PNG/GIF/WebP, max 5MB)
4. File uploads

**Expected:**
- "Uploading..." text appears
- Success toast: "Avatar uploaded successfully!"
- Avatar image updates immediately
- Network: `POST /api/employees/{id}/upload-avatar` returns `200`

**Error Cases to Test:**
- Upload non-image file → Error toast
- Upload file > 5MB → Error toast

#### ✅ Test: Set Employee Status
**UI Steps:**
1. Select an employee
2. Go to "Profile" tab
3. In Status section, click "Set Active" or "Set On Leave"

**Expected:**
- Status badge updates immediately
- Success toast notification
- Network: `POST /api/employees/{id}/set-active` or `set-on-leave` returns `204`

---

### 4. Employee Profile Tab

#### ✅ Test: View Employee Profile
**UI Steps:**
1. Click on any employee in the list
2. Right side shows employee detail
3. "Profile" tab is active by default

**Expected Display:**
- Full Name
- Email
- Phone
- Department
- Position
- Status badge
- "Set Active" / "Set On Leave" buttons (if applicable)

#### ✅ Test: Edit Employee Profile
**UI Steps:**
1. In Profile tab, click "Edit" button
2. Form fields become editable
3. Modify fields
4. Click "Save Changes"

**Expected:**
- Fields become editable
- Success toast on save
- Data updates in employee list
- Network: `PUT /api/employees/{id}` returns `200`

---

### 5. Contracts Tab

#### ✅ Test: GET /api/employees/{employeeId}/contracts
**UI Steps:**
1. Select an employee
2. Click "Contracts" tab
3. Should load and display contracts

**Expected:**
- Network: `GET /api/employees/{id}/contracts` returns `200`
- Displays list of contracts with:
  - Employment Type (Full Time/Part Time/Contract/Internship)
  - Start Date & End Date
  - Salary
  - Status (Active/Ended)

#### ✅ Test: POST /api/employees/{employeeId}/contracts (Create Contract)
**UI Steps:**
1. In Contracts tab, click "Add Contract"
2. Form appears
3. Fill in:
   - Start Date: 2025-01-01
   - End Date: 2025-12-31
   - Employment Type: Full Time
   - Base Salary: 100000
4. Click "Create Contract"

**Expected:**
- Success toast notification
- New contract appears in list
- Form closes
- Network: `POST /api/employees/{id}/contracts` returns `201`

#### ✅ Test: Contract Validation - Overlapping Dates
**UI Steps:**
1. Create a contract: 2025-01-01 to 2025-06-30
2. Try to create another: 2025-03-01 to 2025-09-30 (overlaps!)
3. Click "Create Contract"

**Expected:**
- Error toast with message about overlapping contracts
- Network returns `400` or `422`
- Contract NOT created

#### ✅ Test: Contract Validation - Invalid Dates
**UI Steps:**
1. Try to create contract with End Date before Start Date
2. Example: Start: 2025-12-31, End: 2025-01-01

**Expected:**
- Error toast with validation message
- Network returns error
- Contract NOT created

---

### 6. Leave Requests Tab

#### ✅ Test: GET /api/employees/{employeeId}/leave-requests
**UI Steps:**
1. Select an employee
2. Click "Leave Requests" tab
3. Should load and display leave requests

**Expected:**
- Network: `GET /api/employees/{id}/leave-requests` returns `200`
- Displays:
  - Leave Type (Annual/Sick/Personal/etc.)
  - Start & End Date
  - Reason
  - Status (Draft/Submitted/Approved/etc.)
  - Approver Name (if applicable)

#### ✅ Test: POST /api/employees/{employeeId}/leave-requests (Create Draft)
**UI Steps:**
1. In Leave Requests tab, click "Create Leave Draft"
2. Fill in form:
   - Leave Type: Annual
   - Start Date: 2025-02-01
   - End Date: 2025-02-05
   - Reason: "Vacation"
3. Click "Create Draft"

**Expected:**
- Success toast: "Leave request draft created successfully"
- New leave request appears with status "Draft"
- Network: `POST /api/employees/{id}/leave-requests` returns `201`
- Draft shows "Submit" button

#### ✅ Test: POST /api/leave-requests/{id}/submit (Submit Draft)
**UI Steps:**
1. Find a leave request with status "Draft"
2. Click "Submit" button

**Expected:**
- Success toast: "Leave request submitted"
- Status changes to "Submitted"
- Network: `POST /api/leave-requests/{id}/submit` returns `200`/`204`
- Now shows "Approve", "Reject", "Cancel" buttons

#### ✅ Test: POST /api/leave-requests/{id}/approve
**UI Steps:**
1. Find a leave request with status "Submitted"
2. Click "Approve" button (green)

**Expected:**
- Success toast: "Leave request approved"
- Status changes to "Approved"
- Network: `POST /api/leave-requests/{id}/approve` returns `200`/`204`
- No action buttons (read-only)

#### ✅ Test: POST /api/leave-requests/{id}/reject
**UI Steps:**
1. Find a leave request with status "Submitted"
2. Click "Reject" button (red)

**Expected:**
- Success toast: "Leave request rejected"
- Status changes to "Rejected"
- No action buttons (read-only)

#### ✅ Test: POST /api/leave-requests/{id}/cancel
**UI Steps:**
1. Find a leave request with status "Submitted"
2. Click "Cancel" button (yellow)

**Expected:**
- Success toast: "Leave request cancelled"
- Status changes to "Cancelled"
- No action buttons (read-only)

#### ✅ Test: Leave Request Validation - Overlapping Dates
**UI Steps:**
1. Create leave request: 2025-02-01 to 2025-02-05
2. Try to create another: 2025-02-03 to 2025-02-07 (overlaps!)

**Expected:**
- Error toast about overlapping leave periods
- Leave request NOT created
- Network returns error `400`/`422`

#### ✅ Test: Leave Request Validation - Not in Contract Period
**UI Steps:**
1. Employee has contract from 2025-01-01 to 2025-12-31
2. Try to create leave request for 2026-01-01 to 2026-01-05 (outside contract!)

**Expected:**
- Error toast about dates not within employment contract
- Leave request NOT created
- Network returns error

---

### 7. SignalR Real-time Notifications

#### ✅ Test: SignalR Connection
**UI Steps:**
1. Open Person page
2. Check browser console for "SignalR Connected" message
3. Look at notification panel (bottom-right corner)
4. Green dot = Connected, Red dot = Disconnected

**Expected:**
- Console shows: "SignalR Connected"
- Notification panel shows green connection indicator

#### ✅ Test: EmployeeUpdated Notification
**Trigger:**
1. Open Person page in one browser tab
2. In another tab/window, update an employee

**Expected:**
- Notification panel shows new notification
- Message: "Employee updated: {name}"
- Green badge with employee icon

#### ✅ Test: LeaveRequestUpdated Notification
**Trigger:**
1. Keep Person page open
2. Create, submit, or approve a leave request

**Expected:**
- Real-time notification appears
- Message shows leave request status change

#### ✅ Test: ContractUpdated Notification
**Trigger:**
1. Keep Person page open
2. Create a new contract for an employee

**Expected:**
- Notification appears immediately
- Shows contract update message

#### ✅ Test: ContractExpiringSoon Notification
**Note:** This is triggered by background service checking contracts expiring within 30 days

**Expected:**
- Notification with yellow/warning color
- Message: "Contract expiring for {employee name}"

#### ✅ Test: UpcomingLeave Notification
**Note:** Background service sends notifications for leaves starting within 7 days

**Expected:**
- Blue notification
- Message: "Upcoming leave for {employee name}"

#### ✅ Test: Clear Notifications
**UI Steps:**
1. Have several notifications in panel
2. Click "Clear all" button

**Expected:**
- All notifications removed
- Panel shows "No notifications yet"

#### ✅ Test: Remove Single Notification
**UI Steps:**
1. Click X button on any notification

**Expected:**
- That notification removed
- Others remain

---

## 🔍 Error Handling Tests

### Test Invalid Employee Email
```json
POST /api/employees
{
  "fullName": "Test User",
  "email": "invalid-email",  // Invalid format
  "phone": "123"
}
```
**Expected:** `400` error with validation message

### Test Missing Required Fields
```json
POST /api/employees
{
  "email": "test@example.com"
  // Missing fullName (required)
}
```
**Expected:** `400` error

### Test Invalid Department/Position ID
```json
POST /api/employees
{
  "fullName": "Test",
  "email": "test@example.com",
  "departmentId": 99999  // Non-existent ID
}
```
**Expected:** `400` error: "Department not found"

---

## 📊 UI/UX Verification

### ✅ Modal Background (FIXED)
- Click "New" or "Edit" on employee
- **Expected:** White/light gray background, NOT black
- Modal should be clearly visible with rounded corners

### ✅ Avatar Upload (RESTORED)
- Hover over employee avatar in list
- **Expected:** Upload icon overlay appears
- Click to upload image
- **Expected:** Avatar updates immediately after upload

### ✅ Loading States
- All data fetches show loading spinner
- Buttons show "Loading..." or "Saving..." when processing
- No double-click issues

### ✅ Toast Notifications
- Success: Green toast with success message
- Error: Red toast with error message
- Positioned correctly and dismissible

### ✅ Responsive Layout
- Test on different screen sizes
- Employee list and details should be side-by-side on desktop
- Stack vertically on mobile

### ✅ Tab Navigation
- Click between Profile, Contracts, Leave Requests tabs
- Active tab highlighted in blue
- Content switches correctly

---

## 🎬 Full End-to-End Scenario

### Complete Employee Lifecycle Test:

1. **Create Employee**
   - Click "New" → Fill form → Create
   - ✅ Employee appears in list

2. **Upload Avatar**
   - Hover over avatar → Upload image
   - ✅ Avatar displays

3. **Update Profile**
   - Select employee → Profile tab → Edit → Save
   - ✅ Updates reflected

4. **Add Contract**
   - Contracts tab → Add Contract → Fill dates/salary → Create
   - ✅ Contract appears with "Active" status

5. **Create Leave Request**
   - Leave Requests tab → Create Leave Draft → Fill form → Create
   - ✅ Draft created with "Submit" button

6. **Submit Leave Request**
   - Click "Submit" on draft
   - ✅ Status changes to "Submitted"
   - ✅ SignalR notification appears

7. **Approve Leave**
   - Click "Approve"
   - ✅ Status changes to "Approved"
   - ✅ Another notification

8. **Set On Leave**
   - Profile tab → Click "Set On Leave"
   - ✅ Status badge updates to "On Leave"

9. **Set Active**
   - Click "Set Active"
   - ✅ Back to "Active" status

10. **Delete Employee**
    - Click Delete → Confirm
    - ✅ Employee removed from list

---

## ✅ Success Criteria

All tests passing means:
- ✅ All 5 API groups working (Employees, Departments, Positions, Contracts, Leave Requests)
- ✅ Full CRUD operations functional
- ✅ Business validations working (overlapping contracts/leaves, date validation)
- ✅ State machine transitions correct (Draft→Submitted→Approved/Rejected)
- ✅ SignalR real-time notifications working
- ✅ Avatar upload via Cloudinary working
- ✅ UI/UX improvements (white modal, avatar display)
- ✅ Error handling with friendly messages
- ✅ Loading states and smooth interactions

---

## 🐛 Common Issues & Solutions

### Issue: SignalR not connecting
**Solution:** Check backend is running on port 3000, CORS configured correctly

### Issue: Avatar upload fails
**Solution:** Verify Cloudinary credentials in backend `.env`

### Issue: "Department not found" error
**Solution:** Create departments first via Swagger/Postman before assigning to employees

### Issue: Contract validation errors
**Solution:** Ensure dates don't overlap with existing contracts for same employee

### Issue: Modal has black background
**Solution:** ✅ Already fixed! Should now show light gray/white background

---

## 📝 Testing Notes

- Use browser DevTools Network tab to monitor API calls
- Check Console for errors and SignalR connection logs
- Test with multiple browser tabs to verify real-time updates
- Try edge cases (empty strings, very long names, special characters)
- Verify all error messages are user-friendly

Happy Testing! 🚀
