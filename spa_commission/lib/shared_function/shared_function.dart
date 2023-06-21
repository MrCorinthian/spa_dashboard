class Validator {
  static bool isValidEmail(String email) {
    // Regular expression to validate email format
    final pattern = r'^[\w-]+(\.[\w-]+)*@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,7}$';

    final regex = RegExp(pattern);
    return regex.hasMatch(email);
  }
}
