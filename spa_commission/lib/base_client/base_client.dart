import 'dart:io';
import 'package:http/http.dart' as http;

// const baseUrl = 'http://localhost:49393/api/';
const baseUrl = 'http://192.168.1.150/api/';
// const baseUrl = 'http://192.168.1.150:49393/api/';

class BaseClient {
  final client = http.Client();

  String getBaseUrl = baseUrl;

  Future<dynamic> get(String api) async {
    final url = Uri.parse('$baseUrl$api');
    final response = await http.get(url);
    if (response.statusCode == 200) {
      return response.body;
    } else {
      print('Request failed with status: ${response.statusCode}.');
      return null;
    }
  }

  Future<dynamic> post(String api, dynamic object) async {
    final url = Uri.parse('$baseUrl$api');
    try {
      var response = await client.post(url, body: object);
      if (response.statusCode == 200) {
        return response.body;
      } else {
        print('Request failed with status: ${response.statusCode}.');
        return null;
      }
    } finally {
      client.close();
    }
  }

  Future<dynamic> uploadImage(File? imageFile) async {
    if (imageFile != null) {
      var request = http.MultipartRequest(
          'POST', Uri.parse('${baseUrl}File/UploadImage'));

      var multipartFile =
          await http.MultipartFile.fromPath('image', imageFile.path);
      request.files.add(multipartFile);

      var response = await request.send();
      if (response.statusCode == 200) {
        print('Image uploaded successfully');
      } else {
        print('Image upload failed with status code ${response.statusCode}');
      }
      var responseBody = await response.stream.bytesToString();
      return responseBody;
    }
  }
}
