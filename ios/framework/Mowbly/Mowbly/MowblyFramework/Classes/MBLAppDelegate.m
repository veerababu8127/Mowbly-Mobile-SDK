//
//  MBLAppDelegate.m
//  Mowbly
//
//  Created by Sathish on 08/10/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "ASIHttpRequest.h"
#import "iToast.h"
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLAppBackgroundTasksManager.h"
#import "MBLAppForegroundTasksManager.h"
#import "MBLLogger.h"
#import "MBLPage.h"
#import "MBLRootViewController.h"
#import "MBLURLProtocol.h"
#import "UncaughtExceptionHandler.h"
#import "MBLLogger.h"


@interface MBLAppDelegate ()

@property (nonatomic, retain) MBLPageViewController *topViewController;

- (void)installUncaughtExceptionHandler;

@end

@implementation MBLAppDelegate

@synthesize window = _window,
            navigationController = _navigationController,
            pageViewController =_pageViewController,
            topViewController = _topViewController;

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    
    
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    self.navigationController = [[UINavigationController alloc]init];
    // Set the UINavigationControllerDelegate
    [[self navigationController] setDelegate:self];
    
    // Hide the navigation bar
	[self.navigationController setNavigationBarHidden:YES];
    
    //init RVC
    
    [self.navigationController setViewControllers:[NSArray arrayWithObject:[self allocRVC]]];
    [self.window setRootViewController:self.navigationController];
    
    // Make view visible
    [self.window makeKeyAndVisible];
    
    [NSURLProtocol registerClass:[MBLURLProtocol class]];
    
    return YES;
} // application:didFinishLaunchingWithOptions

- (MBLRootViewController *)allocRVC {
    
    MBLRootViewController *rvc = [[MBLRootViewController alloc] init];
    return rvc;
    
} // init RVC

- (void)applicationWillResignActive:(UIApplication *)application {

    [[self appBackgroundTasksManager] suspendTasks];
}

- (void)applicationDidEnterBackground:(UIApplication *)application {
    
    NSString *jsMessage = @"try { __mowbly__._onBackground(); } catch(e){}";
    MBLPageViewController *pvc = [self appViewController];
    MBLPage *page = pvc.activePage;
    [page pushJavascriptMessage:jsMessage];
} // applicationDidEnterBackground:

- (void)applicationWillEnterForeground  :(UIApplication *)application {

    NSString *jsMessage = @"try { __mowbly__._onForeground(); } catch(e){}";
    MBLPageViewController *pvc = [self appViewController];
    MBLPage *page = pvc.activePage;
    [page pushJavascriptMessage:jsMessage];
} // applicationWillEnterForeground:

- (void)applicationDidBecomeActive:(UIApplication *)application {

    [[self appForegroundTasksManager] runTasks];
} // applicationDidBecomeActive:

- (void)applicationWillTerminate:(UIApplication *)application {

} // applicationWillTerminate:

+ (NSBundle *)frameworkBundle {
    
    static NSBundle* frameworkBundle = nil;
    static dispatch_once_t predicate;
    dispatch_once(&predicate, ^{
        NSString* mainBundlePath = [[NSBundle mainBundle] resourcePath];
        NSString* frameworkBundlePath = [mainBundlePath stringByAppendingPathComponent:@"Mowbly.bundle"];
        frameworkBundle = [NSBundle bundleWithPath:frameworkBundlePath];
    });
    return frameworkBundle;
    
} //framework bundle

#pragma mark -
#pragma mark UINavigationControllerDelegate methods

-(void) navigationController:(UINavigationController *)navigationController
       didShowViewController:(UIViewController *)viewController
                    animated:(BOOL)animated {
    
    if([viewController isKindOfClass:[MBLPageViewController class]]) {
        
        MBLPageViewController *pvc = (MBLPageViewController *)viewController;
        NSArray *aViewControllers = [[self navigationController] viewControllers];
        if(! [aViewControllers containsObject:[self topViewController]]) {
            
            // Pop
            if([self topViewController]) {

                NSDictionary *aCachedViewControllers = [[MBLApp viewControllers] copy];
                [aCachedViewControllers enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {
                    
                    if(! [aViewControllers containsObject:obj]) {
                        
                        [MBLApp removeViewController:obj];
                    }
                }];
            }
        }
        
        [self setTopViewController:pvc];
    }
} // navigationController:didShowViewController:animated:

#pragma mark -
#pragma mark MBLAppDelegate methods

- (void)initApp {

    //Changing default useragent
    NSDictionary *infoDict = [NSDictionary dictionaryWithContentsOfFile:[[[self class] frameworkBundle] pathForResource:@"Mowbly-Info" ofType:@"plist"]];

    NSString *deviceType = [UIDevice currentDevice].model;
    NSString *currentOSVersion = [[UIDevice currentDevice] systemVersion];
    NSString *appName = infoDict [@"MBLAppName"];
    NSString *version = infoDict [@"CFBundleVersion"];
    NSString *parentApp = infoDict [@"MBLFramework"];
    NSString *parentAppVersion = infoDict [@"MBLFrameworkVersion"];
    UIWebView *webView = [[UIWebView alloc]init];
    NSDictionary *dictionary = [NSDictionary dictionaryWithObjectsAndKeys:[NSString stringWithFormat:@"%@/%@ %@/%@ %@/%@ %@::%@",appName,version,parentApp,parentAppVersion,@"iOS",currentOSVersion,deviceType,[webView stringByEvaluatingJavaScriptFromString:@"navigator.userAgent"]] , @"UserAgent", nil];
    
    //ASIHTTP request user agent change
    [ASIHTTPRequest setDefaultUserAgentString:[dictionary valueForKey:@"UserAgent"]];
    
    [[NSUserDefaults standardUserDefaults] registerDefaults:dictionary];
    NSString *plistFile = [self plistFile:infoDict];
    // Get the plist file
    if (! plistFile) {

        NSLog(@"Properties file not specified in Info plist. Missing MBLPropertiesFile key in Info.plist.");
    }
    
    // Get the strings file
    NSString *slistFile = [infoDict valueForKey:@"MBLStringsFile"];

    // Get the plist file
    if (! slistFile) {

        NSLog(@"Strings file not specified in Info plist. Missing MBLStrings key in Info.plist.");
    }

    [MBLApp initWithPropertiesFile:plistFile andStringsFile:slistFile];

    // Configure loggers
    [self configLoggers];
    
} // initApp

- (NSString *)plistFile:(NSDictionary *)options {
    
    return [options objectForKey:@"MBLPropertiesFile"];
} // return plist filepath

- (void) configLoggers {
    
    [MBLLogger configureDefaultLoggers];
} //  configLoggers

- (MBLAppForegroundTasksManager *)appForegroundTasksManager {
    
    return [MBLAppForegroundTasksManager defaultManager];
} // appForegroundTasksManager

- (MBLAppBackgroundTasksManager *)appBackgroundTasksManager {
    
    return [MBLAppBackgroundTasksManager defaultManager];
} // appForegroundTasksManager

- (MBLPageViewController *)appViewController {

    return [self pageViewController];
} // appViewController

- (NSUInteger)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window {

    if ([window.rootViewController isKindOfClass:[UINavigationController class]]) {

        if ([self.navigationController.visibleViewController isEqual:[[self.navigationController viewControllers] objectAtIndex:0]]) {

            return UIInterfaceOrientationMaskPortrait;
        }
    }
    return UIInterfaceOrientationMaskAllButUpsideDown;
} // application:supportedInterfaceOrientationsForWindow:


#pragma mark -
#pragma mark MBLAppDelegate private methods

- (void)installUncaughtExceptionHandler {

	InstallUncaughtExceptionHandler();

    iToast *toast = [[iToast alloc] initWithText:@"Debug mode enabled"];
    [toast show];
} // installUncaughtExceptionHandler

@end