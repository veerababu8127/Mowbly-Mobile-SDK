//
//  MBLRootViewController.m
//  Mowbly
//
//  Created by Sathish on 07/06/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLLogger.h"
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLPage.h"
#import "MBLProgress.h"
#import "MBLRootViewController.h"
#import "MBLURLProtocol.h"

@implementation UINavigationController (Rotation)

-(BOOL)shouldAutorotate {

    return [self.topViewController shouldAutorotate];
} // shouldAutoRotate

-(NSUInteger)supportedInterfaceOrientations {

    return [self.topViewController supportedInterfaceOrientations];
} // supportedInterfaceOrientations

- (UIInterfaceOrientation)preferredInterfaceOrientationForPresentation {

    return [self.topViewController preferredInterfaceOrientationForPresentation] || UIInterfaceOrientationPortrait;
} // preferredInterfaceOrientationForPresentation

@end

@implementation MBLRootViewController

@synthesize splashScreenView,progressText,progressBar;
#pragma mark -
#pragma mark UIApplicationDelegate methods

// Called when the view loading is completed.
- (void)viewDidLoad {
    [super viewDidLoad];
    
    CGRect screenRect = [[UIScreen mainScreen] bounds];
    
    //Adding splash screen to the view controller
    self.splashScreenView = [[UIImageView alloc]initWithFrame:screenRect];
    [self.view addSubview:splashScreenView];
    
    //Adding progressbar to the viewcontroller
    self.progressBar = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    CGFloat y = self.view.center.y;
    y+=50.0;
    CGPoint center = CGPointMake(self.view.center.x, y);
    self.progressBar.center = center;
    [self.view addSubview:self.progressBar];
    
    //Adding progress text to the view controller
    y = screenRect.size.height/1.3;
    self.progressText = [[UITextView alloc]initWithFrame:CGRectMake(0, y, screenRect.size.width, 100)];
    [self.progressText setBackgroundColor:[UIColor clearColor]];
    
    NSDictionary* infoDict = [NSDictionary dictionaryWithContentsOfFile:[[MBLAppDelegate frameworkBundle] pathForResource:@"Mowbly-Info" ofType:@"plist"]];

    [self.progressText setTextColor:[self colorFromHexString:[infoDict objectForKey:@"MBLTextColor"]]];
    self.progressText.textAlignment = NSTextAlignmentCenter;
    self.progressText.text = @"Launching...";
    [self.view addSubview:self.progressText];
    
    if (screenRect.size.height == 568) {
        
        [splashScreenView setImage:[UIImage imageWithContentsOfFile:[[MBLAppDelegate frameworkBundle] pathForResource:@"Default-568h@2x" ofType:@"png"]]];
    } else if(screenRect.size.height < 568) {
        
        [splashScreenView setImage:[UIImage imageWithContentsOfFile:[[MBLAppDelegate frameworkBundle] pathForResource:@"Default" ofType:@"png"]]];
    } else {
        
        [splashScreenView setImage:[UIImage imageWithContentsOfFile:[[MBLAppDelegate frameworkBundle] pathForResource:@"Default-Portrait~ipad" ofType:@"png"]]];
    }
    [self showSplashScreen];
    // Bootstrap
    [self bootstrap];
    
    //Launch home page
    [self launchApp];
    [self preferredStatusBarStyle];
} // viewDidLoad

-(UIStatusBarStyle)preferredStatusBarStyle{
    return UIStatusBarStyleLightContent;
}


- (UIColor *)colorFromHexString:(NSString *)hexString {
    unsigned rgbValue = 0;
    NSScanner *scanner = [NSScanner scannerWithString:hexString];
    [scanner setScanLocation:1]; // bypass '#' character
    [scanner scanHexInt:&rgbValue];
    return [UIColor colorWithRed:((rgbValue & 0xFF0000) >> 16)/255.0 green:((rgbValue & 0xFF00) >> 8)/255.0 blue:(rgbValue & 0xFF)/255.0 alpha:1.0];
}

// Override to allow orientations other than the default portrait orientation.
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {

    return (interfaceOrientation == UIInterfaceOrientationPortrait);
} // shouldAutorotateToInterfaceOrientation

- (void)viewWillAppear:(BOOL)animated {
    
    [[self navigationController] setNavigationBarHidden:YES];
} // viewDidAppear

- (void)viewWillDisappear:(BOOL)animated {
    
    [[self navigationController] setNavigationBarHidden:NO];
} // viewDidDisappear

#pragma mark -
#pragma mark MBLRootViewController private methods

- (void)bootstrap {
    // Initialize objects
    appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
    // Initialize the App
    [appDelegate initApp];
    
} // bootstrap

- (NSString *)installationFailedMessagewithError:(NSError *)error {

    return [MBLLocalizedString(@"INSTALLATION_FAILED") stringByAppendingFormat:@"Reason - %@", [error localizedDescription]];
} // installationFailedMessagewithError:

- (void) showSplashScreen {

    if (progressBar && [progressBar isHidden]) {

        [progressBar startAnimating];
    }
    if(progressText  && [progressText isHidden]) {

        [progressText setHidden:NO];
    }
    if(splashScreenView  && [splashScreenView isHidden]) {

        [splashScreenView setHidden:NO];
    }
} // showSplashScreen

- (void)hideSplashScreen {

	// Hide the splash screen, progressbar and progresstext and release the views
    if(splashScreenView) {

        [splashScreenView setHidden:YES];
    }

    if (progressBar) {

        [progressBar stopAnimating];
    }

    if(progressText) {

        [progressText setHidden:YES];
    }
} // hideSplashScreen

- (void) setInstallationProgress:(NSString *)message {

    [progressText setText:MBLLocalizedString(message)];
} // setInstallationProgress

- (void) stopProgressBar {

    // Stop the progress bar
    if (progressBar) {

        [progressBar stopAnimating];
    }
} // stopProgressBar

#pragma mark - 
#pragma mark MBLRootViewController methods

- (void)launchAppWithName:(NSString *)name url:(NSString *)url {
    
    [[MBLPageViewController alloc] initWithHome:name url:url];
    [self.navigationController pushViewController:[appDelegate pageViewController] animated:YES];
    
} // launchAppWithName:url:

- (void)launchDefaultApp:(BOOL)reload {

    if(! reload) {

        [self launchHomeApp];
        
    } else {
        
        /* Present next run loop. Prevents "unbalanced VC display" warnings. */
        double delayInSeconds = 1;
        dispatch_time_t popTime = dispatch_time(DISPATCH_TIME_NOW, delayInSeconds * NSEC_PER_SEC);
        dispatch_after(popTime, dispatch_get_main_queue(), ^(void) {

            [self hideSplashScreen];
        });
    }
} // launchDefaultApp

- (void)launchApp {
    [self launchHomeApp];
}

- (void)launchHomeApp {
    
    [self launchAppWithName:MBLProperty(@"HOME_PAGE_TITLE")
                        url:MBLProperty(@"HOME_PAGE_URL")];
} // launchHomeApp


#pragma mark -
#pragma mark MBLConnectivityDelegate methods

- (void)didNetworkStatusUpdate:(BOOL)connected {

    MBLPageViewController *pvc = [appDelegate pageViewController];
    if(pvc) {

        [pvc didNetworkStatusUpdate:connected];
    }
} // didNetworkStatusUpdate:


@end    