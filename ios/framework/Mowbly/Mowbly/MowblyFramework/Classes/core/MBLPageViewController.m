//
//  MBLPageViewController.m
//  Mowbly
//
//  Created by Sathish on 30/03/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "CJSONDeserializer.h"
#import "CJSONSerializer.h"
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLConnectivity.h"
#import "MBLFeatureResult.h"
#import "MBLJSMessage.h"
#import "MBLPage.h"
#import "MBLPageViewController.h"
#import "MBLUtils.h"
#import <QuartzCore/QuartzCore.h>

@interface MBLPageViewController (private)

@end

@implementation MBLPageViewController

@synthesize cid = _cid,
            retainControllerOnLowMemory = _retainControllerOnLowMemory,
            activePage = _activePage,
            homeName = _homeName,
            homeUrl = _homeUrl,
            homeData = _homeData,
            pages, pageStack;

-(id) init {
    
    if((self = [super init])) {
        
        // Register controller for native-js bridge
        [self setCid:[MBLUtils GUID]];
        [MBLApp addViewController:self];
        
        // Initialize variables
        appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
        [self setPages:[NSMutableDictionary dictionary]];
        pageStack = [[NSMutableArray alloc] init];
        
        CGRect screenRect = [[UIScreen mainScreen] bounds];
        _screenWidth = screenRect.size.width;
        _screenHeight = screenRect.size.height;

        // Set pageViewController on appDelegate
        [appDelegate setPageViewController:self];
    }
    
    return self;
} // init

- (id)initWithHome:(NSString *)theHomeName
               url:(NSString *)theHomeUrl{

    return [self initWithHomeAndData:theHomeName url:theHomeUrl homeData: nil];
}
- (id)initWithHomeAndData:(NSString *)theHomeName
                      url:(NSString *)theHomeUrl
                 homeData:(NSString *)theHomeData{

    if ((self = [self init])) {

        [self setHomeName:theHomeName];
        [self setHomeUrl:theHomeUrl];
        [self setHomeData:theHomeData];
    }
    return self;
}

#pragma mark -
#pragma mark View lifecycle

- (void)viewDidLoad {

    [super viewDidLoad];

    if([self respondsToSelector:@selector(edgesForExtendedLayout)])
        self.edgesForExtendedLayout = UIRectEdgeNone;

    // Set pageViewController on appDelegate
    [appDelegate setPageViewController:self];
    [self openHome];
    
} // viewDidLoad


- (void) openHome {
    
    [self openPageWithName:[self homeName] url:[self homeUrl] data:[self homeData] configuration:nil retainPageInViewStack:YES direction:ANIM_IN];
} // open home page

- (void)viewDidLayoutSubviews {

    if ([self respondsToSelector:@selector(topLayoutGuide)]) {

        CGRect viewBounds = self.view.bounds;
        CGFloat topBarOffset = self.topLayoutGuide.length;
        viewBounds.origin.y = topBarOffset * -1;
        self.view.bounds = viewBounds;
    }
} // viewDidLayoutSubviews

- (void)viewWillAppear:(BOOL)animated {
    
    // Set pageViewController on appDelegate
    [appDelegate setPageViewController:self];
    
    [[self navigationController] setNavigationBarHidden:YES];
} // viewDidAppear

- (void)viewWillDisappear:(BOOL)animated {

    // Clear pageViewController on appDelegate
    [appDelegate setPageViewController:nil];

    [[self navigationController] setNavigationBarHidden:NO];
} // viewDidDisappear

- (void)setView:(UIView*)view {
    
    // view is set to nil by the system during low memory conditions.
    // bRetainPage is set when the page needs to wait for any results, say for Camera, thus preventing view set to nil.
    if(! _retainControllerOnLowMemory) {

        [super setView:view];
    }
} // setView

- (BOOL)shouldAutorotate {

    return YES;
} // shouldAutorotate

- (NSUInteger)supportedInterfaceOrientations {

    return UIInterfaceOrientationMaskAllButUpsideDown;
} // supportedInterfaceOrientations

- (UIInterfaceOrientation)preferredInterfaceOrientationForPresentation {
    
    return UIInterfaceOrientationPortrait;
} // preferredInterfaceOrientationForPresentation

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // TODO need to check for previous orientation because it'll always call this method
    BOOL allowOrientationChange = (interfaceOrientation!=UIInterfaceOrientationPortraitUpsideDown);
    return allowOrientationChange;
} // shouldAutoRotateToInterfaceOrientation

- (void)willAnimateRotationToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation duration:(NSTimeInterval)duration
{
    CGFloat screenHeight, screenWidth;
    
    // Get the screen dimensions based on orientation
    CGRect statusBarRect = [[UIApplication sharedApplication] statusBarFrame];
    if (UIDeviceOrientationIsPortrait([UIApplication sharedApplication].statusBarOrientation)) {
        
        screenWidth = _screenWidth;
        screenHeight = _screenHeight;
        screenHeight = (screenHeight - statusBarRect.size.height);
    } else {
        
        screenWidth = _screenHeight;
        screenHeight = _screenWidth;
        screenHeight = (screenHeight - statusBarRect.size.width);
    }
    [_activePage setFrame:CGRectMake(0, 0, screenWidth, screenHeight)];
    
    NSString *msg = [NSString stringWithFormat:@"window.__mowbly__._onDeviceOrientationChange(%d, %d, %f, %f);",[self interfaceOrientation], YES, screenWidth, screenHeight];
    [_activePage pushJavascriptMessage:msg];
} // willAnimateToInterfaceOrientation:

#pragma mark -
#pragma mark View methods

- (MBLPage *)getPageByName:(NSString *)pageName {

    return [[self pages] objectForKey:pageName];
} // getViewByName

- (MBLPage *)openPageWithName:(NSString *)pageName 
                          url:(NSString *)url
                         data:(NSString *)pageData
                 configuration:(NSDictionary *)configuration
        retainPageInViewStack:(BOOL)bRetainPageInPageStack
                    direction:(AnimationDirection)direction {
    BOOL bPageResumed = NO;
    BOOL bSetPrevPageToParent = YES;
    NavigationDirection navDir = (direction == ANIM_IN) ? FORWARD : BACKWARD;

    if (_activePage && ![_activePage retainInViewStack]) {

        [self.pages removeObjectForKey:_activePage.name];
        [pageStack removeObject:_activePage.name];
        [_activePage removeFromSuperview];
    }

    prevPage = _activePage;
    _activePage = [[self pages] objectForKey:pageName];

    if (! _activePage) {

        // Create the page object and assign to activepage
        
        [self createPageWithName:pageName configuration:configuration];

        // Set the view controller
        [_activePage setPvc:self];

        // Add it to the pages collection
        [[self pages] setObject:_activePage forKey:pageName];

        // Set Retain in page stack property
        [_activePage setRetainInViewStack:bRetainPageInPageStack];

        // Add to page stack
        [pageStack addObject:pageName];
    } else {

        // Mark resumed
        bPageResumed = YES;

        // If page is in page stack, navigate to it; else add to page stack.
        __block BOOL bPageInViewStack = NO;
        __block int index = -1;
        [pageStack enumerateObjectsUsingBlock:^(id obj, NSUInteger idx, BOOL *stop) {

            index++;
            if([obj isEqualToString:pageName]) {

                *stop = bPageInViewStack = YES;
            }
        }]; 

        if(bPageInViewStack) {

            // Remove all stack entries till the page
            for(int i=([pageStack count]-1); i>index; i--) {

                [pageStack removeObjectAtIndex:i];
            }

            // Call to open a page in the page stack. NOT a back press.
            if(direction == ANIM_IN) {

                bSetPrevPageToParent = YES;

                // Reset the direction for animation
                direction = ANIM_OUT;
            }
        } else {

            [pageStack addObject:pageName];
        }
    }
    if(prevPage) {

        // Resign active state
        [prevPage setIsActive:NO];

        if(direction == ANIM_IN) {

            // Set previous page as parent
            [_activePage setParentPage:[prevPage retainInViewStack] ? [prevPage name] : [prevPage parentPage]];
        }
    }

    // Set page data, if the page is opened new/resumed.
    if(navDir == FORWARD) {

        [_activePage setData:pageData];
    }

    // Load the url
    if(url && ![url isEqualToString:@""] && ! [[[[[_activePage request] URL] absoluteString] stringByReplacingPercentEscapesUsingEncoding:NSUTF8StringEncoding] hasSuffix:url]) {
        
        url = [self getPageUrl:url name:pageName];
    
        [_activePage loadUrl:url];

        // New Page load. Resuming invalid.
        bPageResumed = NO;
    }

    // Set the current page as active
    [_activePage setIsActive:YES];

    // Resume page
    if(bPageResumed) {

        [_activePage onPageLoad:(navDir == FORWARD) ? NO : YES];
    }

    if(prevPage) {

        // Do the transition
        [self transitionFromPage:prevPage
                          toPage:_activePage
                       direction:direction 
                        duration:0.10];
        
        if(bSetPrevPageToParent) {

            prevPage = [[self pages] objectForKey:[_activePage parentPage]];
        }
    } else {

        if (![_activePage superview]) {

            [self.view addSubview:_activePage];
        }
    }
    return _activePage;
} // openPageWithName:url:data:configuration:retainPageInViewStack:direction

- (void) createPageWithName:(NSString *)pageName configuration:(NSDictionary *)theConfiguration {
    
    _activePage = [[MBLPage alloc] initWithPageName:pageName configuration:theConfiguration];
}

- (NSString *) getPageUrl:(NSString *)url name:(NSString *)pageName {
    
    NSString *resourcePath = [[NSBundle mainBundle] resourcePath];
    NSDictionary *infoDict = [NSDictionary dictionaryWithContentsOfFile:[[MBLAppDelegate frameworkBundle] pathForResource:@"Mowbly-Info" ofType:@"plist"]];
    url = [NSString stringWithFormat:@"%@/%@/%@%@",resourcePath,[infoDict valueForKey:@"MBLBundleName"],[infoDict valueForKey:@"MBLProjectNamespace"],url];
    url = [[NSURL fileURLWithPath:url
                      isDirectory:NO] absoluteString];
    return url;
    
} // set the url

- (void) reloadActivePage {

    [_activePage reload];
} // reloadActivePage

- (void)navigateBack:(BOOL)destroy {

    if([pageStack count] > 1) {

        // Remove the page from the page stack
        int index = ([pageStack count] - 1);
        [pageStack removeObjectAtIndex:index--];

        // Open the previous page
        MBLPage *previousPage = [[self pages] objectForKey:[pageStack objectAtIndex:index]];
        while(! [previousPage retainInViewStack] && index > 0) {
            
            // Remove the page from the view, as it is not retained in the page stack
            [pageStack removeObjectAtIndex:index--];
            previousPage = [[self pages] objectForKey:[pageStack objectAtIndex:index]];
        }
        
        if(destroy) {
            
            [[self pages] removeObjectForKey:[_activePage name]];
            [_activePage removeFromSuperview];
        } else {

            // Remains as a cached version. Remove parent until added again to page stack.
            [_activePage setParentPage:nil];
        }
        [self openPageWithName:[previousPage name] url:nil data:nil configuration:nil retainPageInViewStack:[previousPage retainInViewStack] direction:ANIM_OUT];
    } else {

        // If there are other view controllers in custom app, they can be shown.
        // MBLRootViewController never has visual elements of app except for installation elements
        // Do not go back if this controller is the first one after Root view controller
        if([[[self navigationController] viewControllers] count] > 2) {

            [[self navigationController] popViewControllerAnimated:YES];
        }
    }
} // navigateBack

#pragma mark -
#pragma mark Animations

- (void)transitionFromPage:(MBLPage *)fromPage toPage:(MBLPage *)toPage direction:(AnimationDirection)direction duration:(float)duration {

    if (!toPage.superview) {

        [self.view addSubview:toPage];
    }
    
    CGRect toViewFrame;
    CGFloat screenHeight, screenWidth;
    
    // Get the screen dimensions based on orientation
    CGRect statusBarRect = [[UIApplication sharedApplication] statusBarFrame];
    if (UIDeviceOrientationIsPortrait([UIApplication sharedApplication].statusBarOrientation)) {

        screenWidth = _screenWidth;
        screenHeight = _screenHeight;
        screenHeight = (screenHeight - statusBarRect.size.height);
    } else {

        screenWidth = _screenHeight;
        screenHeight = _screenWidth;
        screenHeight = (screenHeight - statusBarRect.size.width);
    }
    
    CGFloat startY = 0.0;
    CGFloat pageHeight = screenHeight;
    
    if(direction == ANIM_IN) {

        // Set frame for this page offscreen
        toPage.frame = CGRectMake(screenWidth, startY, screenWidth, pageHeight);
    } else {

        // Set frame for this page offscreen
        toPage.frame = CGRectMake(0-screenWidth, startY, screenWidth, pageHeight);
    }
    
    // Set new frame
    toViewFrame = CGRectMake(0.0, startY, screenWidth, pageHeight);
    
    // Do the animation
    [UIView animateWithDuration:duration+0.270
                     animations:^{
                         
                         toPage.frame = toViewFrame;
                     }];
    
    if (fromPage.superview) {

        [fromPage removeFromSuperview];
    }
} // transitionFromView:toView:direction:duration

#pragma mark -
#pragma mark MBLConnectivityDelegate methods

- (void)didNetworkStatusUpdate:(BOOL)connected {
    
    NSString *jsMessage;
    // Raise onConnect and onDisconnect events to JS layer
    if (connected) {
        int nw = [[MBLConnectivity getInstance] networkStatus];
        NSNumber *code = [NSNumber numberWithInt:nw];
        NSString *response = [[MBLConnectivity getInstance] networkType];
        
        MBLFeatureResult *result = [MBLFeatureResult resultWithCode:code result:response error:Nil];
        
        jsMessage = [NSString stringWithFormat:@"try { __mowbly__.Network.onConnect(%@); } catch(e){}", [result toJSONString]];
    } else {
        jsMessage = @"try { __mowbly__.Network.onDisconnect(); } catch(e){}";
    }
    
    [pages enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {
        
        MBLPage *page = (MBLPage*)obj;
        [page pushJavascriptMessage:jsMessage];
    }];
} //didNetworkStatusUpdate:

@end