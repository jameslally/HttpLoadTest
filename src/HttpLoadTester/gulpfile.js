'use strict';

var gulp = require('gulp'),
    tsc = require('gulp-typescript'),
    tslint = require('gulp-tslint'),
    sourcemaps = require('gulp-sourcemaps'),
    del = require('del'),
    project = require('./project.json'),
    tsProject = tsc.createProject('tsconfig.json'),
    rimraf = require('gulp-rimraf'),
    concat = require('gulp-concat'),
    cssmin = require('gulp-cssmin'),
    uglify = require('gulp-uglify'),
    rename = require('gulp-rename'),
    bower = require('gulp-bower');

var webroot = './' + project.webroot + '/', 
    bowerDirectory = "./bower_components/";

var paths = {
    js: [
        project.tsOutputPath + 'json2.js',
        project.tsOutputPath + 'jquery.cookie.js',
        project.tsOutputPath + 'signalr.samples.js',
        project.tsOutputPath + 'DashboardCanvasJS.js',
        project.tsOutputPath + 'Dashboard.js'
    ],
    jsRoot: webroot + 'js',
    jsLib:
    [
        bowerDirectory + 'signalr/jquery.signalR.js',
        bowerDirectory + 'handlebars/handlebars.js',
        bowerDirectory + 'handlebars-helpers/src/helpers.js',
        bowerDirectory + 'handlebars-helper-intl/dist/handlebars-intl.js',
        bowerDirectory + 'chart.js/dist/Chart.js',
        bowerDirectory + 'moment/moment.js',
    ],
    jsLibNoBundle:
    [
        bowerDirectory + 'jquery/dist/jquery.js',
        bowerDirectory + 'bootstrap/dist/js/bootstrap.js'
    ],
    cssRoot: webroot + 'css',
    cssLibNoBundle:
    [
        bowerDirectory + 'bootstrap/dist/css/bootstrap.css',
        bowerDirectory + 'bootstrap/dist/css/bootstrap.min.css'
    ]
    
};

/*********************************
 * typescript startTag
**********************************/

gulp.task('ts-lint', function () {
    return gulp.src(project.tsSource)
          .pipe(tslint({
          formatter: "verbose"
      }))
      .pipe(tslint.report());       
});

/**
 * Compile TypeScript and include references to library and app .d.ts files.
 */
gulp.task('compile-ts', function () {
    var sourceTsFiles = [project.tsSource,                //path to typescript files
                         project.tsLlbraryDefinitions]; //reference to library .d.ts files
                        

    var tsResult = gulp.src(sourceTsFiles)
                       .pipe(sourcemaps.init())
                       .pipe(tsProject());



        tsResult.dts.pipe(gulp.dest(project.tsOutputPath));
        return tsResult.js
                        .pipe(sourcemaps.write('.'))
                        .pipe(gulp.dest(project.tsOutputPath));
});

/**
 * Remove all generated JavaScript files from TypeScript compilation.
 */
gulp.task('clean-ts', function (cb) {
  var typeScriptGenFiles = [
                              project.tsOutputPath +'**/*.js',    // path to all JS files auto gen'd by editor
                              project.tsOutputPath +'**/*.js.map', // path to all sourcemap files auto gen'd by editor
                              '!' + project.tsOutputPath + 'lib'
                           ];

  // delete the files
  del(typeScriptGenFiles, cb);
});


/*********************************
 * typescript endtag
**********************************/

gulp.task('clean:js', function () {
    //gulp.src([paths.jsBuildMin], { read: false })
    //    .pipe(rimraf({ force: true }));
});

gulp.task('clean:css', function () {
    //gulp.src([paths.cssBuildMin, '!' + paths.cssBuild], { read: false })
    //    .pipe(rimraf({ force: true }));
});

gulp.task('clean', ['clean:js', 'clean:css'], function () { });

gulp.task('min:js', ['clean:js'], function () {
    var jsPath = paths.jsLib.concat(paths.js);
    gulp.src(jsPath)
        .pipe(concat('site.js'))
        .pipe(gulp.dest(paths.jsRoot))
        .pipe(uglify())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(paths.jsRoot))
    ;
});

gulp.task('copy:js', function () {
    var jsPath = paths.jsLib.concat(paths.jsLibNoBundle);
    gulp.src(jsPath)
        .pipe(gulp.dest(paths.jsRoot + '/lib'))
        .pipe(uglify())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(paths.jsRoot + '/lib'))
    ;
});

gulp.task('copy:css', function () {
    gulp.src(paths.cssLibNoBundle)
        .pipe(gulp.dest(paths.cssRoot + '/lib'))
    ;
});

/*******************************************
 * Tasks
 */
gulp.task('bower', function() {
    return bower()
        .pipe(gulp.dest(bowerDirectory))});


gulp.task('watch', function() {
    gulp.watch([project.tsSource], ['ts-lint', 'compile-ts']);
});

gulp.task('default', ['ts-lint', 'compile-ts', 'copy:js', 'min:js' , 'copy:css']);

/****************************
 * 
 ***************************/