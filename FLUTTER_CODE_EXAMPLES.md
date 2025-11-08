# Flutter Code Examples for Hello Doctor API

## Project Setup

### 1. Create Flutter Project

```bash
flutter create hello_doctor_app
cd hello_doctor_app
```

### 2. Add Dependencies

Update `pubspec.yaml`:

```yaml
dependencies:
  flutter:
    sdk: flutter

  # HTTP client
  http: ^1.1.0

  # Secure storage for tokens
  flutter_secure_storage: ^9.0.0

  # State management (choose one)
  provider: ^6.1.1
  # OR riverpod: ^2.4.9
  # OR flutter_bloc: ^8.1.3

  # Image picker for prescription upload
  image_picker: ^1.0.4

  # File picker
  file_picker: ^6.1.1

  # WebView for payments
  webview_flutter: ^4.4.2

  # JSON serialization
  json_annotation: ^4.8.1

  # Intl for date formatting
  intl: ^0.18.1

dev_dependencies:
  flutter_test:
    sdk: flutter
  build_runner: ^2.4.6
  json_serializable: ^6.7.1
```

---

## Project Structure

```
lib/
├── main.dart
├── models/
│   ├── user.dart
│   ├── beneficiary.dart
│   ├── prescription.dart
│   └── payment.dart
├── services/
│   ├── api_client.dart
│   ├── auth_service.dart
│   ├── prescription_service.dart
│   └── payment_service.dart
├── providers/
│   └── auth_provider.dart
├── screens/
│   ├── auth/
│   │   ├── login_screen.dart
│   │   └── register_screen.dart
│   ├── home/
│   │   └── home_screen.dart
│   ├── beneficiaries/
│   │   ├── beneficiaries_screen.dart
│   │   └── add_beneficiary_screen.dart
│   ├── prescriptions/
│   │   ├── prescriptions_screen.dart
│   │   ├── upload_prescription_screen.dart
│   │   └── prescription_details_screen.dart
│   └── payments/
│       ├── payment_screen.dart
│       └── payment_history_screen.dart
└── widgets/
    └── custom_widgets.dart
```

---

## 1. Models

### `lib/models/user.dart`

```dart
import 'package:json_annotation/json_annotation.dart';

part 'user.g.dart';

@JsonSerializable()
class User {
  final String id;
  final String email;
  final String firstName;
  final String lastName;
  final String? phoneNumber;
  final String role;

  User({
    required this.id,
    required this.email,
    required this.firstName,
    required this.lastName,
    this.phoneNumber,
    required this.role,
  });

  factory User.fromJson(Map<String, dynamic> json) => _$UserFromJson(json);
  Map<String, dynamic> toJson() => _$UserToJson(this);

  String get fullName => '$firstName $lastName';
}

@JsonSerializable()
class AuthResponse {
  final User? user;
  final String? token;
  final String? refreshToken;
  final DateTime? expiresAt;

  AuthResponse({
    this.user,
    this.token,
    this.refreshToken,
    this.expiresAt,
  });

  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    // Handle the API response structure
    if (json['value'] != null) {
      return AuthResponse(
        user: User.fromJson({
          'id': json['value']['id'],
          'email': json['value']['email'],
          'firstName': json['value']['firstName'],
          'lastName': json['value']['lastName'],
          'phoneNumber': json['value']['phoneNumber'],
          'role': json['value']['role'],
        }),
        token: json['value']['token'],
        refreshToken: json['value']['refreshToken'],
        expiresAt: DateTime.parse(json['value']['expiresAt']),
      );
    }
    return AuthResponse();
  }
}
```

### `lib/models/beneficiary.dart`

```dart
import 'package:json_annotation/json_annotation.dart';

part 'beneficiary.g.dart';

@JsonSerializable()
class Beneficiary {
  final int id;
  final String firstName;
  final String lastName;
  final String idNumber;
  final DateTime dateOfBirth;
  final String gender;
  final String relationship;
  final String? email;
  final String? phoneNumber;

  Beneficiary({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.idNumber,
    required this.dateOfBirth,
    required this.gender,
    required this.relationship,
    this.email,
    this.phoneNumber,
  });

  factory Beneficiary.fromJson(Map<String, dynamic> json) =>
      _$BeneficiaryFromJson(json);
  Map<String, dynamic> toJson() => _$BeneficiaryToJson(this);

  String get fullName => '$firstName $lastName';
}
```

### `lib/models/prescription.dart`

```dart
import 'package:json_annotation/json_annotation.dart';

part 'prescription.g.dart';

@JsonSerializable()
class Prescription {
  final int id;
  final int beneficiaryId;
  final String? beneficiaryName;
  final String? notes;
  final String status;
  final DateTime issuedDate;
  final DateTime expiryDate;
  final List<PrescriptionFile>? files;

  Prescription({
    required this.id,
    required this.beneficiaryId,
    this.beneficiaryName,
    this.notes,
    required this.status,
    required this.issuedDate,
    required this.expiryDate,
    this.files,
  });

  factory Prescription.fromJson(Map<String, dynamic> json) =>
      _$PrescriptionFromJson(json);
  Map<String, dynamic> toJson() => _$PrescriptionToJson(this);

  bool get isExpired => DateTime.now().isAfter(expiryDate);
  bool get requiresPayment => status == 'PaymentPending';
}

@JsonSerializable()
class PrescriptionFile {
  final int id;
  final String fileName;
  final String fileUrl;
  final String fileType;

  PrescriptionFile({
    required this.id,
    required this.fileName,
    required this.fileUrl,
    required this.fileType,
  });

  factory PrescriptionFile.fromJson(Map<String, dynamic> json) =>
      _$PrescriptionFileFromJson(json);
  Map<String, dynamic> toJson() => _$PrescriptionFileToJson(this);
}
```

### `lib/models/payment.dart`

```dart
import 'package:json_annotation/json_annotation.dart';

part 'payment.g.dart';

@JsonSerializable()
class Payment {
  final int paymentId;
  final String status;
  final double amount;
  final String currency;
  final String purpose;
  final String? provider;
  final int? prescriptionId;
  final DateTime initiatedAt;
  final DateTime? completedAt;

  Payment({
    required this.paymentId,
    required this.status,
    required this.amount,
    required this.currency,
    required this.purpose,
    this.provider,
    this.prescriptionId,
    required this.initiatedAt,
    this.completedAt,
  });

  factory Payment.fromJson(Map<String, dynamic> json) =>
      _$PaymentFromJson(json);
  Map<String, dynamic> toJson() => _$PaymentToJson(this);

  bool get isCompleted => status == 'Completed';
  bool get isPending => status == 'Pending';
  bool get hasFailed => status == 'Failed';
}

@JsonSerializable()
class InitiatePaymentResponse {
  final int paymentId;
  final String paymentUrl;
  final String status;

  InitiatePaymentResponse({
    required this.paymentId,
    required this.paymentUrl,
    required this.status,
  });

  factory InitiatePaymentResponse.fromJson(Map<String, dynamic> json) {
    final value = json['value'];
    return InitiatePaymentResponse(
      paymentId: value['paymentId'],
      paymentUrl: value['paymentUrl'],
      status: value['status'],
    );
  }
}
```

---

## 2. Services

### `lib/services/api_client.dart`

```dart
import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ApiClient {
  static const String baseUrl = 'https://your-api-domain.com/api/v1';
  final _storage = const FlutterSecureStorage();

  // Singleton pattern
  static final ApiClient _instance = ApiClient._internal();
  factory ApiClient() => _instance;
  ApiClient._internal();

  String? _token;
  DateTime? _tokenExpiry;

  Future<void> setToken(String token, DateTime expiresAt) async {
    _token = token;
    _tokenExpiry = expiresAt;
    await _storage.write(key: 'jwt_token', value: token);
    await _storage.write(key: 'token_expiry', value: expiresAt.toIso8601String());
  }

  Future<void> loadToken() async {
    _token = await _storage.read(key: 'jwt_token');
    final expiryStr = await _storage.read(key: 'token_expiry');
    if (expiryStr != null) {
      _tokenExpiry = DateTime.parse(expiryStr);
    }
  }

  Future<void> clearToken() async {
    _token = null;
    _tokenExpiry = null;
    await _storage.delete(key: 'jwt_token');
    await _storage.delete(key: 'token_expiry');
    await _storage.delete(key: 'refresh_token');
  }

  bool get isTokenExpired {
    if (_tokenExpiry == null) return true;
    return DateTime.now().isAfter(_tokenExpiry!.subtract(const Duration(minutes: 5)));
  }

  Map<String, String> get _headers => {
        'Content-Type': 'application/json',
        if (_token != null) 'Authorization': 'Bearer $_token',
      };

  Future<Map<String, dynamic>> get(String endpoint) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl$endpoint'),
        headers: _headers,
      );

      return _handleResponse(response);
    } catch (e) {
      throw ApiException('Network error: $e');
    }
  }

  Future<Map<String, dynamic>> post(
    String endpoint,
    Map<String, dynamic> body,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl$endpoint'),
        headers: _headers,
        body: json.encode(body),
      );

      return _handleResponse(response);
    } catch (e) {
      throw ApiException('Network error: $e');
    }
  }

  Future<Map<String, dynamic>> put(
    String endpoint,
    Map<String, dynamic> body,
  ) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl$endpoint'),
        headers: _headers,
        body: json.encode(body),
      );

      return _handleResponse(response);
    } catch (e) {
      throw ApiException('Network error: $e');
    }
  }

  Future<Map<String, dynamic>> delete(String endpoint) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl$endpoint'),
        headers: _headers,
      );

      return _handleResponse(response);
    } catch (e) {
      throw ApiException('Network error: $e');
    }
  }

  Future<Map<String, dynamic>> uploadFile(
    String endpoint,
    Map<String, String> fields,
    List<http.MultipartFile> files,
  ) async {
    try {
      final request = http.MultipartRequest(
        'POST',
        Uri.parse('$baseUrl$endpoint'),
      );

      // Add headers
      request.headers.addAll({
        if (_token != null) 'Authorization': 'Bearer $_token',
      });

      // Add fields
      request.fields.addAll(fields);

      // Add files
      request.files.addAll(files);

      final streamedResponse = await request.send();
      final response = await http.Response.fromStream(streamedResponse);

      return _handleResponse(response);
    } catch (e) {
      throw ApiException('Upload error: $e');
    }
  }

  Map<String, dynamic> _handleResponse(http.Response response) {
    final body = json.decode(response.body);

    if (response.statusCode >= 200 && response.statusCode < 300) {
      return body;
    } else if (response.statusCode == 401) {
      throw UnauthorizedException('Unauthorized');
    } else if (response.statusCode == 403) {
      throw ForbiddenException('Forbidden');
    } else if (response.statusCode == 404) {
      throw NotFoundException('Not found');
    } else {
      final errors = body['errors'] ?? ['Unknown error'];
      throw ApiException(errors.first);
    }
  }
}

class ApiException implements Exception {
  final String message;
  ApiException(this.message);

  @override
  String toString() => message;
}

class UnauthorizedException extends ApiException {
  UnauthorizedException(String message) : super(message);
}

class ForbiddenException extends ApiException {
  ForbiddenException(String message) : super(message);
}

class NotFoundException extends ApiException {
  NotFoundException(String message) : super(message);
}
```

### `lib/services/auth_service.dart`

```dart
import '../models/user.dart';
import 'api_client.dart';

class AuthService {
  final ApiClient _apiClient = ApiClient();

  Future<AuthResponse> login(String email, String password) async {
    final response = await _apiClient.post('/authentication/login', {
      'email': email,
      'password': password,
    });

    final authResponse = AuthResponse.fromJson(response);

    if (authResponse.token != null && authResponse.expiresAt != null) {
      await _apiClient.setToken(authResponse.token!, authResponse.expiresAt!);
    }

    return authResponse;
  }

  Future<bool> register({
    required String email,
    required String password,
    required String confirmPassword,
    required String firstName,
    required String lastName,
    required String phoneNumber,
    required String idNumber,
    required DateTime dateOfBirth,
    required String gender,
    required String address,
  }) async {
    final response = await _apiClient.post('/authentication/create', {
      'email': email,
      'password': password,
      'confirmPassword': confirmPassword,
      'firstName': firstName,
      'lastName': lastName,
      'phoneNumber': phoneNumber,
      'idNumber': idNumber,
      'dateOfBirth': dateOfBirth.toIso8601String(),
      'gender': gender,
      'address': address,
      'role': 'MainMember',
    });

    return response['value'] == true;
  }

  Future<void> logout() async {
    await _apiClient.clearToken();
  }

  Future<User> getUserProfile(String userId) async {
    final response = await _apiClient.get('/authentication/get-user-by-id/$userId');
    return User.fromJson(response['value']);
  }
}
```

### `lib/services/prescription_service.dart`

```dart
import 'dart:io';
import 'package:http/http.dart' as http;
import '../models/prescription.dart';
import 'api_client.dart';

class PrescriptionService {
  final ApiClient _apiClient = ApiClient();

  Future<int> uploadPrescription({
    required int beneficiaryId,
    required DateTime issuedDate,
    required DateTime expiryDate,
    String? notes,
    required List<File> files,
  }) async {
    // Prepare multipart files
    final multipartFiles = <http.MultipartFile>[];
    for (final file in files) {
      multipartFiles.add(
        await http.MultipartFile.fromPath(
          'Files',
          file.path,
        ),
      );
    }

    // Prepare form fields
    final fields = {
      'BeneficiaryId': beneficiaryId.toString(),
      'IssuedDate': issuedDate.toIso8601String(),
      'ExpiryDate': expiryDate.toIso8601String(),
      if (notes != null) 'Notes': notes,
    };

    final response = await _apiClient.uploadFile(
      '/prescription/upload-file',
      fields,
      multipartFiles,
    );

    return response['value'];
  }

  Future<Prescription> getPrescriptionDetails(int prescriptionId) async {
    final response = await _apiClient.get('/prescription/$prescriptionId');
    return Prescription.fromJson(response['value']);
  }
}
```

### `lib/services/payment_service.dart`

```dart
import '../models/payment.dart';
import 'api_client.dart';

class PaymentService {
  final ApiClient _apiClient = ApiClient();

  Future<InitiatePaymentResponse> initiatePayment({
    required double amount,
    required int prescriptionId,
    String? notes,
  }) async {
    final response = await _apiClient.post('/payment/initiate', {
      'amount': amount,
      'purpose': 0, // PrescriptionFee
      'provider': 0, // PayFast
      'prescriptionId': prescriptionId,
      if (notes != null) 'notes': notes,
    });

    return InitiatePaymentResponse.fromJson(response);
  }

  Future<Payment> getPaymentStatus(int paymentId) async {
    final response = await _apiClient.get('/payment/$paymentId/status');
    return Payment.fromJson(response['value']);
  }

  Future<List<Payment>> getPaymentHistory({int page = 1, int pageSize = 20}) async {
    final response = await _apiClient.get('/payment/history?page=$page&pageSize=$pageSize');
    final List<dynamic> paymentsList = response['value'];
    return paymentsList.map((json) => Payment.fromJson(json)).toList();
  }
}
```

---

## 3. Sample Screens

### `lib/screens/auth/login_screen.dart`

```dart
import 'package:flutter/material.dart';
import '../../services/auth_service.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({Key? key}) : super(key: key);

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _authService = AuthService();
  bool _isLoading = false;

  Future<void> _login() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    try {
      final authResponse = await _authService.login(
        _emailController.text.trim(),
        _passwordController.text,
      );

      if (!mounted) return;

      if (authResponse.user != null) {
        // Navigate to home screen
        Navigator.pushReplacementNamed(context, '/home');
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Login failed: $e')),
      );
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Login')),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              TextFormField(
                controller: _emailController,
                decoration: const InputDecoration(
                  labelText: 'Email',
                  border: OutlineInputBorder(),
                ),
                keyboardType: TextInputType.emailAddress,
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter your email';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _passwordController,
                decoration: const InputDecoration(
                  labelText: 'Password',
                  border: OutlineInputBorder(),
                ),
                obscureText: true,
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter your password';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 24),
              SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton(
                  onPressed: _isLoading ? null : _login,
                  child: _isLoading
                      ? const CircularProgressIndicator()
                      : const Text('Login'),
                ),
              ),
              const SizedBox(height: 16),
              TextButton(
                onPressed: () {
                  Navigator.pushNamed(context, '/register');
                },
                child: const Text('Don\'t have an account? Register'),
              ),
            ],
          ),
        ),
      ),
    );
  }

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }
}
```

### `lib/screens/payments/payment_screen.dart`

```dart
import 'package:flutter/material.dart';
import 'package:webview_flutter/webview_flutter.dart';
import '../../services/payment_service.dart';
import '../../models/payment.dart';

class PaymentScreen extends StatefulWidget {
  final int prescriptionId;
  final double amount;

  const PaymentScreen({
    Key? key,
    required this.prescriptionId,
    required this.amount,
  }) : super(key: key);

  @override
  State<PaymentScreen> createState() => _PaymentScreenState();
}

class _PaymentScreenState extends State<PaymentScreen> {
  final _paymentService = PaymentService();
  InitiatePaymentResponse? _paymentResponse;
  bool _isLoading = true;
  String? _error;
  late final WebViewController _webViewController;

  @override
  void initState() {
    super.initState();
    _initiatePayment();
  }

  Future<void> _initiatePayment() async {
    try {
      final response = await _paymentService.initiatePayment(
        amount: widget.amount,
        prescriptionId: widget.prescriptionId,
        notes: 'Payment for prescription #${widget.prescriptionId}',
      );

      setState(() {
        _paymentResponse = response;
        _isLoading = false;
      });

      // Initialize WebView controller
      _webViewController = WebViewController()
        ..setJavaScriptMode(JavaScriptMode.unrestricted)
        ..setNavigationDelegate(
          NavigationDelegate(
            onNavigationRequest: (NavigationRequest request) {
              // Check if payment completed (returned to return URL)
              if (request.url.contains('/payment/return/')) {
                _checkPaymentStatus();
                return NavigationDecision.prevent;
              }
              // Check if payment cancelled
              if (request.url.contains('/payment/cancel/')) {
                Navigator.pop(context, false);
                return NavigationDecision.prevent;
              }
              return NavigationDecision.navigate;
            },
          ),
        )
        ..loadRequest(Uri.parse(response.paymentUrl));
    } catch (e) {
      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  Future<void> _checkPaymentStatus() async {
    if (_paymentResponse == null) return;

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => const Center(
        child: CircularProgressIndicator(),
      ),
    );

    try {
      // Poll for payment status (max 10 times, 2 seconds apart)
      for (int i = 0; i < 10; i++) {
        await Future.delayed(const Duration(seconds: 2));

        final payment = await _paymentService.getPaymentStatus(
          _paymentResponse!.paymentId,
        );

        if (payment.isCompleted) {
          if (!mounted) return;
          Navigator.pop(context); // Close loading dialog
          Navigator.pop(context, true); // Return to previous screen with success
          return;
        } else if (payment.hasFailed) {
          if (!mounted) return;
          Navigator.pop(context); // Close loading dialog
          _showError('Payment failed. Please try again.');
          return;
        }
      }

      // Timeout
      if (!mounted) return;
      Navigator.pop(context);
      _showError('Payment status check timed out. Please check payment history.');
    } catch (e) {
      if (!mounted) return;
      Navigator.pop(context);
      _showError('Error checking payment status: $e');
    }
  }

  void _showError(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message)),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Payment'),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.error, size: 64, color: Colors.red),
                      const SizedBox(height: 16),
                      Text(_error!),
                      const SizedBox(height: 16),
                      ElevatedButton(
                        onPressed: () => Navigator.pop(context),
                        child: const Text('Go Back'),
                      ),
                    ],
                  ),
                )
              : _paymentResponse != null
                  ? WebViewWidget(controller: _webViewController)
                  : const Center(child: Text('No payment URL')),
    );
  }
}
```

---

## 4. Generate Model Code

After creating the model files, run:

```bash
flutter pub get
flutter pub run build_runner build --delete-conflicting-outputs
```

This generates the `.g.dart` files for JSON serialization.

---

## 5. Main App Setup

### `lib/main.dart`

```dart
import 'package:flutter/material.dart';
import 'screens/auth/login_screen.dart';
import 'screens/auth/register_screen.dart';
import 'screens/home/home_screen.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Hello Doctor',
      theme: ThemeData(
        primarySwatch: Colors.blue,
        useMaterial3: true,
      ),
      initialRoute: '/login',
      routes: {
        '/login': (context) => const LoginScreen(),
        '/register': (context) => const RegisterScreen(),
        '/home': (context) => const HomeScreen(),
      },
    );
  }
}
```

---

## Tips for Development

### 1. Error Handling

```dart
try {
  final result = await someApiCall();
  // Handle success
} on UnauthorizedException {
  // Clear tokens and redirect to login
  await AuthService().logout();
  Navigator.pushReplacementNamed(context, '/login');
} on ApiException catch (e) {
  // Show error message to user
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(content: Text(e.message)),
  );
}
```

### 2. Loading States

```dart
bool _isLoading = false;

Future<void> _loadData() async {
  setState(() => _isLoading = true);
  try {
    // API call
  } finally {
    setState(() => _isLoading = false);
  }
}
```

### 3. File Upload with Progress

```dart
import 'package:image_picker/image_picker.dart';

Future<void> _pickAndUploadImage() async {
  final ImagePicker picker = ImagePicker();
  final XFile? image = await picker.pickImage(source: ImageSource.gallery);

  if (image != null) {
    final file = File(image.path);
    // Upload file
    await PrescriptionService().uploadPrescription(
      beneficiaryId: 1,
      issuedDate: DateTime.now(),
      expiryDate: DateTime.now().add(const Duration(days: 365)),
      files: [file],
    );
  }
}
```

---

## Next Steps

1. **Clone and setup Flutter project**
2. **Update `baseUrl` in `ApiClient` to your API domain**
3. **Test authentication flow**
4. **Implement remaining screens**
5. **Add state management (Provider/Riverpod/Bloc)**
6. **Test payment flow with PayFast sandbox**
7. **Add error handling and loading states**
8. **Implement offline support**
9. **Add push notifications**
10. **Test on real devices**

---

**Happy Coding!** 🚀
