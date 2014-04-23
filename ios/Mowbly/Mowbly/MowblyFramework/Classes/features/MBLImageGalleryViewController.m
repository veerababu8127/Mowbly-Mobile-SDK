//  MBLImageGallery.m
//  Mowbly
//
//  Created by Veerababu on 09/12/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
#import "MBLImageGalleryViewController.h"
#import "MBLPhotoViewController.h"
#import "MBLAppDelegate.h"

@interface MBLImageGalleryViewController(){
    NSMutableDictionary *_dict;
    NSInteger pageNo,page;
    NSUInteger initialIndex;
}
@end

@implementation MBLImageGalleryViewController

@synthesize pvc;

-(id)initDict:(NSMutableDictionary *)dict{
    if(self=[super init]){
        _dict=[[NSMutableDictionary alloc]initWithDictionary:dict];
        initialIndex=[[dict objectForKey:@"index"] intValue];
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    MBLPhotoViewController *pageZero = [MBLPhotoViewController photoViewControllerForPageIndex:initialIndex withData:_dict];
    if (pageZero != nil)
    {
        pvc = [[UIPageViewController alloc]
               initWithTransitionStyle:1
               navigationOrientation:UIPageViewControllerNavigationOrientationHorizontal
               options:nil];
        pvc.dataSource = self;
        [[pvc view] setFrame:[self view].bounds];
        [self addChildViewController:[self pvc]];
        [[self view] addSubview:[[self pvc] view]];
        [self.pvc didMoveToParentViewController:self];
        [pvc setWantsFullScreenLayout:NO];
        [pvc setViewControllers:@[pageZero]
                      direction:UIPageViewControllerNavigationDirectionForward
                       animated:NO
                     completion:NULL];
    }
    self.view.backgroundColor=[UIColor blackColor];
    NSDictionary *dict=[[_dict objectForKey:@"images"] objectAtIndex:0];
    if ([dict valueForKey:@"title"]!=Nil) {
        self.title=[dict valueForKey:@"title"];
    }
}

-(void)viewWillAppear:(BOOL)animated{
    if (floor(NSFoundationVersionNumber) > NSFoundationVersionNumber_iOS_6_1) {
        self.edgesForExtendedLayout = UIRectEdgeNone;
        [self.navigationController setNavigationBarHidden:NO];
    }
}

- (UIViewController *)pageViewController:(UIPageViewController *)pvc viewControllerBeforeViewController:(MBLPhotoViewController *)vc
{
    NSUInteger index = vc.pageIndex;
    pageNo=index;
    page=0;
    [self setPageTitle];
    return [MBLPhotoViewController photoViewControllerForPageIndex:(index - 1) withData:_dict];
}
- (UIViewController *)pageViewController:(UIPageViewController *)pvc viewControllerAfterViewController:(MBLPhotoViewController *)vc
{
    NSUInteger index = vc.pageIndex;
    pageNo=index;
    page=1;
    [self setPageTitle];
    return [MBLPhotoViewController photoViewControllerForPageIndex:(index + 1) withData:_dict];
}
-(void)setPageTitle
{
    NSDictionary *dict=[[_dict objectForKey:@"images"] objectAtIndex:pageNo];
    if (page==1) {
        if ([dict valueForKey:@"title"]!=Nil) {
            self.title=[dict valueForKey:@"title"];
            }
    }else if(page==0){
        if ([dict valueForKey:@"title"]!=Nil) {
            self.title=[dict valueForKey:@"title"];
        }
    }
}
- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}
- (NSUInteger)supportedInterfaceOrientations {
    
    return UIInterfaceOrientationMaskAllButUpsideDown;

} // supportedInterfaceOrientations
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    BOOL allowOrientationChange = (interfaceOrientation!=UIInterfaceOrientationPortraitUpsideDown);
    return allowOrientationChange;
} // shouldAutoRotateToInterfaceOrientation

@end

