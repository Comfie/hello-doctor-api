# Hello Doctor API - Flutter Mobile App Integration Guide

## Base URLs

- **Development**: `http://localhost:5000` or `https://localhost:5001`
- **Production**: `https://your-production-domain.com`

## API Version

Current API Version: **v1**

All endpoints are prefixed with `/api/v1/`

---

## Authentication

### JWT Bearer Token Authentication

All authenticated endpoints require a JWT token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### 1. Register New Main Member

**Endpoint**: `POST /api/v1/authentication/create`

**Description**: Register a new main member account

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "YourPassword123!",
  "confirmPassword": "YourPassword123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+27821234567",
  "idNumber": "9001015009087",
  "dateOfBirth": "1990-01-01T00:00:00Z",
  "gender": "Male",
  "address": "123 Main Street, Cape Town",
  "role": "MainMember"
}
```

**Response**: `200 OK`
```json
{
  "value": true,
  "isSuccess": true
}
```

---

### 2. Login

**Endpoint**: `POST /api/v1/authentication/login`

**Description**: Authenticate user and receive JWT token

**Request Body**:
```json
{
  "email": "user@example.com",
  "password": "YourPassword123!"
}
```

**Response**: `200 OK`
```json
{
  "value": {
    "id": "user-id-string",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "MainMember",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh-token-string",
    "expiresAt": "2025-11-08T10:00:00Z"
  },
  "isSuccess": true
}
```

**Store these values securely**:
- `token` - Use for Authorization header in all subsequent requests
- `refreshToken` - Use to get a new token when current token expires
- `expiresAt` - Check before each request to see if token needs refreshing

---

### 3. Refresh Token

**Endpoint**: `POST /api/v1/authentication/refresh-token`

**Description**: Get a new access token using refresh token

**Request Body**:
```json
{
  "refreshToken": "your-refresh-token"
}
```

**Response**: `200 OK`
```json
{
  "value": {
    "id": "user-id-string",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "role": "MainMember",
    "token": "new-jwt-token",
    "refreshToken": "new-refresh-token",
    "expiresAt": "2025-11-08T11:00:00Z"
  },
  "isSuccess": true
}
```

---

### 4. Get User Profile

**Endpoint**: `GET /api/v1/authentication/get-user-by-id/{userId}`

**Description**: Get user details by ID

**Headers**:
```
Authorization: Bearer <token>
```

**Response**: `200 OK`
```json
{
  "value": {
    "id": "user-id",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+27821234567",
    "idNumber": "9001015009087",
    "dateOfBirth": "1990-01-01T00:00:00Z",
    "gender": "Male",
    "address": "123 Main Street, Cape Town"
  },
  "isSuccess": true
}
```

---

### 5. Update User Profile

**Endpoint**: `PUT /api/v1/authentication/update-user`

**Description**: Update user profile information

**Headers**:
```
Authorization: Bearer <token>
```

**Request Body**:
```json
{
  "id": "user-id",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+27821234567",
  "address": "456 New Street, Cape Town"
}
```

**Response**: `200 OK`

---

## Beneficiary Management

### 1. Get All Beneficiaries

**Endpoint**: `GET /api/v1/beneficiary/get-all-beneficiaries`

**Description**: Get all beneficiaries for logged-in main member

**Headers**:
```
Authorization: Bearer <token>
```

**Response**: `200 OK`
```json
{
  "value": [
    {
      "id": 1,
      "firstName": "Jane",
      "lastName": "Doe",
      "idNumber": "0505055009088",
      "dateOfBirth": "2005-05-05T00:00:00Z",
      "gender": "Female",
      "relationship": "Daughter",
      "mainMemberId": "user-id"
    }
  ],
  "isSuccess": true
}
```

---

### 2. Get Beneficiary by ID

**Endpoint**: `GET /api/v1/beneficiary/get-beneficiary/{id}`

**Description**: Get specific beneficiary details

**Headers**:
```
Authorization: Bearer <token>
```

**Response**: `200 OK`

---

### 3. Create Beneficiary

**Endpoint**: `POST /api/v1/beneficiary/create`

**Description**: Add a new beneficiary (e.g., child, spouse)

**Headers**:
```
Authorization: Bearer <token>
```

**Request Body**:
```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "idNumber": "0505055009088",
  "dateOfBirth": "2005-05-05T00:00:00Z",
  "gender": "Female",
  "relationship": "Daughter",
  "email": "jane.doe@example.com",
  "phoneNumber": "+27821234568"
}
```

**Response**: `200 OK`
```json
{
  "value": 1,
  "isSuccess": true
}
```

---

### 4. Update Beneficiary

**Endpoint**: `PUT /api/v1/beneficiary/update-beneficiary`

**Description**: Update beneficiary information

**Headers**:
```
Authorization: Bearer <token>
```

**Request Body**:
```json
{
  "id": 1,
  "firstName": "Jane",
  "lastName": "Doe",
  "phoneNumber": "+27821234568",
  "email": "jane.updated@example.com"
}
```

**Response**: `200 OK`

---

### 5. Delete Beneficiary

**Endpoint**: `DELETE /api/v1/beneficiary/delete-beneficiary/{id}`

**Description**: Remove a beneficiary

**Headers**:
```
Authorization: Bearer <token>
```

**Response**: `200 OK`

---

## Prescription Management

### 1. Upload Prescription (File Upload)

**Endpoint**: `POST /api/v1/prescription/upload-file`

**Description**: Upload a prescription with files (images/PDFs)

**Headers**:
```
Authorization: Bearer <token>
Content-Type: multipart/form-data
```

**Form Data**:
- `BeneficiaryId` (required): `1`
- `Notes` (optional): `"Prescription for monthly medication"`
- `IssuedDate` (required): `"2025-11-01T00:00:00Z"`
- `ExpiryDate` (required): `"2026-11-01T00:00:00Z"`
- `Files` (required): File(s) - Images or PDFs of prescription

**Response**: `200 OK`
```json
{
  "value": 123,
  "isSuccess": true
}
```

---

### 2. Get Prescription Details

**Endpoint**: `GET /api/v1/prescription/{prescriptionId}`

**Description**: Get detailed prescription information

**Headers**:
```
Authorization: Bearer <token>
```

**Response**: `200 OK`
```json
{
  "value": {
    "id": 123,
    "beneficiaryId": 1,
    "beneficiaryName": "Jane Doe",
    "notes": "Prescription for monthly medication",
    "status": "PaymentPending",
    "issuedDate": "2025-11-01T00:00:00Z",
    "expiryDate": "2026-11-01T00:00:00Z",
    "files": [
      {
        "id": 1,
        "fileName": "prescription.pdf",
        "fileUrl": "/uploads/prescriptions/prescription.pdf",
        "fileType": "application/pdf"
      }
    ],
    "prescriptionNotes": [],
    "statusHistory": [
      {
        "status": "Pending",
        "changedAt": "2025-11-01T10:00:00Z"
      }
    ]
  },
  "isSuccess": true
}
```

**Prescription Statuses**:
- `Pending` - Initially created
- `PaymentPending` - Waiting for payment
- `Approved` - Payment completed, ready for pharmacy
- `UnderReview` - Assigned to pharmacy, being reviewed
- `OnHold` - Requires clarification
- `PartiallyDispensed` - Some items dispensed
- `FullyDispensed` - All items dispensed
- `ReadyForPickup` - Ready for collection
- `Delivered` - Delivered to patient
- `Rejected` - Rejected by pharmacist
- `Cancelled` - Cancelled by user
- `Expired` - Prescription expired

---

## Payment Management

### 1. Initiate Payment

**Endpoint**: `POST /api/v1/payment/initiate`

**Description**: Initiate a payment for prescription

**Headers**:
```
Authorization: Bearer <token>
```

**Request Body**:
```json
{
  "amount": 150.00,
  "purpose": 0,
  "provider": 0,
  "prescriptionId": 123,
  "notes": "Payment for prescription #123"
}
```

**Purpose Enum**:
- `0` = PrescriptionFee
- `1` = DispenseFee
- `2` = DeliveryFee
- `3` = Refund

**Provider Enum**:
- `0` = PayFast
- `1` = Stripe
- `2` = PayPal

**Response**: `200 OK`
```json
{
  "value": {
    "paymentId": 456,
    "paymentUrl": "https://sandbox.payfast.co.za/eng/process?...",
    "status": "Pending"
  },
  "isSuccess": true
}
```

**Next Steps**:
1. Open `paymentUrl` in WebView or browser
2. User completes payment on PayFast
3. PayFast sends webhook to backend
4. Backend updates payment status
5. Prescription automatically approved if payment succeeds

---

### 2. Get Payment Status

**Endpoint**: `GET /api/v1/payment/{paymentId}/status`

**Description**: Check payment status

**Headers**:
```
Authorization: Bearer <token>
```

**Response**: `200 OK`
```json
{
  "value": {
    "paymentId": 456,
    "status": "Completed",
    "amount": 150.00,
    "currency": "ZAR",
    "purpose": "PrescriptionFee",
    "externalTransactionId": "TXN123456",
    "completedAt": "2025-11-08T10:15:00Z",
    "failureReason": null
  },
  "isSuccess": true
}
```

**Payment Statuses**:
- `Pending` - Payment initiated, awaiting completion
- `Completed` - Payment successful
- `Failed` - Payment failed
- `Cancelled` - User cancelled payment
- `Refunded` - Payment refunded

---

### 3. Get Payment History

**Endpoint**: `GET /api/v1/payment/history?page=1&pageSize=20`

**Description**: Get user's payment history with pagination

**Headers**:
```
Authorization: Bearer <token>
```

**Query Parameters**:
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response**: `200 OK`
```json
{
  "value": [
    {
      "paymentId": 456,
      "status": "Completed",
      "amount": 150.00,
      "currency": "ZAR",
      "purpose": "PrescriptionFee",
      "provider": "PayFast",
      "prescriptionId": 123,
      "initiatedAt": "2025-11-08T10:00:00Z",
      "completedAt": "2025-11-08T10:15:00Z"
    }
  ],
  "isSuccess": true
}
```

---

## Error Handling

### Standard Error Response

All errors follow this format:

```json
{
  "value": null,
  "isSuccess": false,
  "status": 2,
  "errors": ["Error message here"],
  "validationErrors": []
}
```

**Common HTTP Status Codes**:
- `200` - Success
- `400` - Bad Request (validation errors)
- `401` - Unauthorized (missing or invalid token)
- `403` - Forbidden (insufficient permissions)
- `404` - Not Found
- `500` - Internal Server Error

---

## Workflow Examples

### Complete Prescription + Payment Flow

```
1. User logs in
   → POST /api/v1/authentication/login
   → Store JWT token

2. User uploads prescription
   → POST /api/v1/prescription/upload-file
   → Get prescriptionId (e.g., 123)
   → Prescription status: "Pending"

3. System requires payment
   → Prescription status changes to "PaymentPending"

4. User initiates payment
   → POST /api/v1/payment/initiate
   → Get paymentUrl and paymentId

5. Open paymentUrl in WebView
   → User completes payment on PayFast
   → PayFast sends webhook to backend

6. Poll for payment status
   → GET /api/v1/payment/{paymentId}/status
   → Status becomes "Completed"

7. Check prescription status
   → GET /api/v1/prescription/{prescriptionId}
   → Status automatically changed to "Approved"
   → System note added: "Payment completed successfully"

8. Prescription ready for pharmacy assignment
   → Pharmacy receives, reviews, and dispenses medication
```

---

## Important Notes

### 1. CORS Configuration
✅ Already configured to allow all origins - mobile apps can connect

### 2. File Uploads
- Maximum file size: **25 MB**
- Supported formats: PDF, JPG, JPEG, PNG
- Use `multipart/form-data` content type

### 3. Date Format
- All dates use **ISO 8601 format**: `2025-11-08T10:00:00Z`
- Timezone: UTC

### 4. Currency
- All amounts in **South African Rand (ZAR)**
- Decimal format: `150.00`

### 5. Token Expiration
- JWT tokens expire after a set period (check `expiresAt` field)
- Implement token refresh logic before expiration
- Store tokens securely (use Flutter Secure Storage)

---

## Security Best Practices

### For Flutter App Development

1. **Secure Token Storage**
   ```dart
   // Use flutter_secure_storage package
   final storage = FlutterSecureStorage();
   await storage.write(key: 'jwt_token', value: token);
   ```

2. **Automatic Token Refresh**
   ```dart
   // Check token expiration before each request
   // Refresh if expiring soon (e.g., within 5 minutes)
   ```

3. **HTTPS Only**
   - Use HTTPS in production
   - Never send tokens over HTTP

4. **Handle 401 Errors**
   - Automatically redirect to login on 401
   - Clear stored tokens

5. **Validate Input**
   - Validate forms on client-side before sending
   - Show user-friendly error messages

---

## Sample Flutter HTTP Client Setup

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class ApiClient {
  static const String baseUrl = 'https://your-api-domain.com/api/v1';

  String? _token;

  void setToken(String token) {
    _token = token;
  }

  Future<Map<String, dynamic>> get(String endpoint) async {
    final response = await http.get(
      Uri.parse('$baseUrl$endpoint'),
      headers: {
        'Authorization': 'Bearer $_token',
        'Content-Type': 'application/json',
      },
    );

    if (response.statusCode == 401) {
      // Handle unauthorized - refresh token or logout
    }

    return json.decode(response.body);
  }

  Future<Map<String, dynamic>> post(String endpoint, Map<String, dynamic> body) async {
    final response = await http.post(
      Uri.parse('$baseUrl$endpoint'),
      headers: {
        'Authorization': 'Bearer $_token',
        'Content-Type': 'application/json',
      },
      body: json.encode(body),
    );

    return json.decode(response.body);
  }
}
```

---

## Next Steps for Flutter Development

1. **Set up project structure**
   - Create models for API responses
   - Set up state management (Provider, Riverpod, Bloc, etc.)
   - Create API service layer

2. **Implement authentication flow**
   - Login screen
   - Registration screen
   - Token management
   - Auto-refresh logic

3. **Build main features**
   - Dashboard/Home screen
   - Beneficiary management
   - Prescription upload with camera
   - Payment integration (WebView for PayFast)
   - Payment history

4. **Add nice-to-haves**
   - Push notifications (when prescription status changes)
   - Offline support
   - File preview before upload
   - Payment status polling

---

## Testing

### Test Accounts

**PayFast Sandbox Credentials**:
- The app will use sandbox mode automatically
- No real money will be charged
- Test cards provided by PayFast documentation

### API Testing Tools

- **Postman**: Import endpoints for testing
- **Swagger UI**: Available at `https://your-api/swagger` (development only)

---

## Support

For questions or issues:
1. Check API response error messages
2. Verify JWT token is valid and not expired
3. Ensure correct HTTP headers are sent
4. Check network connectivity

---

**Document Version**: 1.0
**Last Updated**: November 8, 2025
**API Version**: v1
