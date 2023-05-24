import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../../base_client/base_client.dart';
import './providers/AuthProvider/auth_provider.dart';
import './app_theme//ThemeNotify.dart';
import './app_theme/app_theme.dart';
import './shared_widget/custom_app_bar.dart';
import './pages/login/login.dart';
import './pages/main/home/home.dart';
import './pages/main/report/report.dart';
import './pages/main/profile/profile.dart';
import './shared_widget/custom_profile_widget.dart';
import '../../../models/mobile_user_info.dart';
// import './pages/main/qr_scan.dart';

void main() {
  runApp(
    ChangeNotifierProvider(
      create: (_) => ThemeNotifier(AppTheme.customTheme),
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeNotifier>(
      builder: (context, themeNotifier, child) {
        return ChangeNotifierProvider(
          create: (context) => AuthProvider(),
          child: MaterialApp(
            title: 'SPA Commission',
            theme: themeNotifier.getTheme(),
            home: Consumer<AuthProvider>(
              builder: (context, auth, _) =>
                  auth.isLoggedIn ? const MainPage() : const LoginPage(),
            ),
          ),
        );
      },
    );
  }
}

class MainPage extends StatefulWidget {
  const MainPage({super.key});

  final String title = 'SPA Commission';

  @override
  State<MainPage> createState() => _MainPageState();
}

class _MainPageState extends State<MainPage> {
  int _selectedIndex = 0;
  String _token = '';
  // MobileUserInfo _userInfo = MobileUserInfo();

  @override
  void initState() {
    super.initState();
  }

  static final List<Widget> _widgetOptions = <Widget>[
    HomePage(),
    ReportPage(),
    ProfilePage(),
    // const QrCodePage(),
    // const QrScanPage(),
  ];

  void _onItemTapped(int index) {
    setState(() {
      _selectedIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    //_widgetOptions.elementAt(_selectedIndex)
    return Scaffold(
      // backgroundColor: CustomTheme.backgroundColor,
      appBar: const CustomAppBar(),
      body: Container(
          padding: CustomTheme.paddingPage,
          child: Column(
            children: [
              if (_selectedIndex != 1) const CustomProfileWidget(),
              _widgetOptions.elementAt(_selectedIndex)
            ],
          )),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _selectedIndex,
        backgroundColor: CustomTheme.darkGreyColor,
        selectedItemColor: CustomTheme.primaryColor,
        unselectedItemColor: CustomTheme.fillColor,
        items: const <BottomNavigationBarItem>[
          BottomNavigationBarItem(
            icon: Icon(Icons.home),
            label: 'Home',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.stacked_bar_chart),
            label: 'Report',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.person),
            label: 'Profile',
          ),
          // BottomNavigationBarItem(
          //   icon: Icon(Icons.qr_code),
          //   label: 'QR Code',
          // ),
          // BottomNavigationBarItem(
          //   icon: Icon(Icons.qr_code_scanner),
          //   label: 'QR Scan',
          // ),
        ],
        onTap: _onItemTapped,
      ),
    );
  }
}
