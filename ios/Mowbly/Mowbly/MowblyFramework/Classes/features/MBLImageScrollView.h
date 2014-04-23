
//
//  MBLImageGallery.m
//  Mowbly
//
//  Created by Veerababu on 09/12/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
#import <UIKit/UIKit.h>

@interface MBLImageScrollView : UIScrollView <UIScrollViewDelegate,NSURLConnectionDelegate>

@property (nonatomic) NSUInteger index;
@property(strong,nonatomic)UIImage *originalImage;
@property(strong,nonatomic)UILabel *label;

- (void)displayImage:(UIImage *)image;
- (void)setIndex:(NSUInteger)index dataDictionary:(NSMutableDictionary *)dict;
-(id)initWithLabel;

@end
