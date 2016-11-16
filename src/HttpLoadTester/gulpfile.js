/// <binding AfterBuild='onbuild' Clean='clean' ProjectOpened='default' />
'use strict';

var gulp = require('gulp'),
    rimraf = require('gulp-rimraf'),
    concat = require('gulp-concat'),
    cssmin = require('gulp-cssmin'),
    uglify = require('gulp-uglify'),
    merge = require('merge-stream'),
    project = require('./project.json'),
    rename = require('gulp-rename'),
    sourcemaps = require('gulp-sourcemaps'),
    bower = require('gulp-bower');

var webroot = './' + project.webroot + '/';

var paths = {
    js: [
        webroot + 'js/json2.js',
        webroot + 'js/jquery.cookie.js',
        webroot + 'js/signalr.samples.js',
        webroot + 'js/DashboardCanvasJS.js',
        webroot + 'js/Dashboard.js'
    ],
    jsRoot: webroot + 'js',
    jsWatch: [webroot + 'js/**/*.js', '!' + webroot + 'js/site.js', '!' + webroot + 'js/**/*.min.js'],
    jsMin: webroot + 'js/**/*.min.js',
    jsBuild: webroot + 'js/site.js',
    jsBuildMin: webroot + 'js/site.min.js',
    jsLib:
    [
        webroot + 'lib/signalr/jquery.signalR.js',
        webroot + 'lib/handlebars/handlebars.js',
        webroot + 'lib/handlebars-helpers/src/helpers.js',
        webroot + 'lib/handlebars-helper-intl/dist/handlebars-intl.js',
        webroot + 'lib/chart.js/dist/Chart.js',
    ],
    jsLibNoBundle:
    [
        webroot + 'lib/jquery/dist/jquery.js',
        webroot + 'lib/bootstrap/dist/js/bootstrap.js'
    ],
    css: webroot + 'css/*.css',
    cssBuild: webroot + 'css/site.css',
    cssBuildMin: webroot + 'css/site.min.css',
    bower:webroot + 'lib'
};

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

gulp.task('min:styles', ['clean:css'], function () {
    gulp.src(paths.cssBuild)
        .pipe(gulp.dest('.'))
        .pipe(cssmin())
        .pipe(rename(paths.cssBuildMin))
        .pipe(gulp.dest('.'))
        .pipe(sourcemaps.write('../../sourcemaps'))
    ;
});

gulp.task('watch', [], function () {
    gulp.watch(paths.jsWatch, ['min:js']);
    gulp.watch([paths.css], ['min:styles']);
});

gulp.task('min', ['min:js', 'min:styles'], function () { });

gulp.task('onbuild', ['bower' , 'min', 'copy:js'], function () { });

gulp.task('default', ['onbuild', 'watch'], function () { });

gulp.task('bower', function() {
    return bower()
        .pipe(gulp.dest(paths.bower))});
