import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
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
  WidgetsFlutterBinding.ensureInitialized();
  runApp(
    ChangeNotifierProvider(
      create: (_) => ThemeNotifier(AppTheme.customTheme),
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatefulWidget {
  const MyApp({super.key});

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    await Future.delayed(
        const Duration(seconds: 2),
        () => setState(() {
              _loading = false;
            }));
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<ThemeNotifier>(
      builder: (context, themeNotifier, child) {
        return ChangeNotifierProvider(
          create: (context) => AuthProvider(),
          child: MaterialApp(
            title: 'Urban Thai Spa',
            theme: themeNotifier.getTheme(),
            home: Consumer<AuthProvider>(
              builder: (context, auth, _) => _loading
                  ? LandingPage()
                  : (auth.isLoggedIn ? const MainPage() : const LoginPage()),
            ),
          ),
        );
      },
    );
  }
}

class LandingPage extends StatefulWidget {
  const LandingPage({super.key});

  final String title = 'Urban Thai Spa';

  @override
  State<LandingPage> createState() => _LandingPageState();
}

class _LandingPageState extends State<LandingPage> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
          child: Column(
        crossAxisAlignment: CrossAxisAlignment.center,
        mainAxisAlignment: MainAxisAlignment.center,
        children: const [
          Image(
            image: AssetImage('assets/images/urban-logo.png'),
            width: 300.00,
          ),
          CircularProgressIndicator(
            color: CustomTheme.primaryColor,
          ),
        ],
      )),
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
  bool _loading = true;
  // MobileUserInfo _userInfo = MobileUserInfo();

  @override
  void initState() {
    super.initState();
    _loadValue();
  }

  Future<void> _loadValue() async {
    // await Future.delayed(
    //     Duration(seconds: 1),
    //     () => setState(() {
    //           _loading = false;
    //         }));
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

  // Widget buildLoadingScreen() => const Scaffold(
  //       body: Center(
  //         child: CircularProgressIndicator(
  //           color: CustomTheme.primaryColor,
  //         ),
  //       ),
  //     );

  @override
  Widget build(BuildContext context) {
    //_widgetOptions.elementAt(_selectedIndex)
    return Scaffold(
      // backgroundColor: CustomTheme.backgroundColor,
      appBar: const CustomAppBar(),
      body: SingleChildScrollView(
        child: Container(
            padding: CustomTheme.paddingPage,
            child: Column(
              children: [
                // if (_selectedIndex != 1) const CustomProfileWidget(),
                _widgetOptions.elementAt(_selectedIndex)
              ],
            )),
      ),
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
        ],
        onTap: _onItemTapped,
      ),
    );
  }
}
