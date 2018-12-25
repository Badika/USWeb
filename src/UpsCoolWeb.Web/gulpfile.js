const gulp = require('gulp');
const rimraf = require('rimraf');
const less = require('gulp-less');
const concat = require('gulp-concat');
const cssmin = require('gulp-cssmin');
const uglify = require('gulp-uglify');

gulp.task('clean.css', (callback) => {
    return rimraf('./wwwroot/content/**/*.min.css', callback);
});

gulp.task('clean.js', (callback) => {
    return rimraf('./wwwroot/scripts/**/*.min.js', callback);
});

gulp.task('clean', gulp.series([
    'clean.css',
    'clean.js'
]));

gulp.task('less', () => {
    return gulp
        .src('./wwwroot/content/**/*.less')
        .pipe(less())
        .pipe(gulp.dest(file => {
            return file.base;
        }));
});

gulp.task('vendor.private.css', () => {
    return gulp
        .src([
            './wwwroot/content/jqueryui/*.css',
            './wwwroot/content/bootstrap/*.css',
            './wwwroot/content/fontawesome/*.css',
            './wwwroot/content/mvcgrid/*.css',
            './wwwroot/content/mvctree/*.css',
            './wwwroot/content/mvclookup/*.css'
        ])
        .pipe(concat('./wwwroot/content/Private/vendor.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('.'));
});

gulp.task('site.private.css', () => {
    return gulp
        .src([
            './wwwroot/content/shared/alerts.css',
            './wwwroot/content/shared/content.css',
            './wwwroot/content/shared/header.css',
            './wwwroot/content/shared/navigation.css',
            './wwwroot/content/shared/overrides.css',
            './wwwroot/content/shared/table.css',
            './wwwroot/content/shared/widget-box.css',
            './wwwroot/content/shared/private.css',
        ])
        .pipe(concat('./wwwroot/content/Private/site.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('.'));
});

gulp.task('vendor.public.css', () => {
    return gulp
        .src([
            './wwwroot/content/bootstrap/*.css',
            './wwwroot/content/fontawesome/*.css'
        ])
        .pipe(concat('./wwwroot/content/Public/vendor.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('.'));
});

gulp.task('site.public.css', () => {
    return gulp
        .src([
            './wwwroot/content/shared/alerts.css',
            './wwwroot/content/shared/content.css',
            './wwwroot/content/shared/overrides.css',
            './wwwroot/content/shared/public.css'
        ])
        .pipe(concat('./wwwroot/content/Public/site.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest('.'));
});

gulp.task('app.css', () => {
    return gulp
        .src(['./wwwroot/content/application/**/*.css', '!./wwwroot/content/application/**/*.min.css'])
        .pipe(cssmin())
        .pipe(gulp.dest(file => {
            file.basename = file.basename.split('.', 1)[0] + '.min.css';

            return file.base;
        }));
});

gulp.task('vendor.private.js', () => {
    return gulp
        .src([
            './wwwroot/scripts/jquery/jquery.js',
            './wwwroot/scripts/jquery/**/*.js',
            './wwwroot/scripts/jqueryui/jquery-ui.js',
            './wwwroot/scripts/jqueryui/**/*.js',
            './wwwroot/scripts/mvclookup/**/*.js',
            './wwwroot/scripts/mvcgrid/**/*.js',
            './wwwroot/scripts/mvctree/*.js',
            './wwwroot/scripts/bootstrap/*.js'
        ])
        .pipe(concat('./wwwroot/scripts/Private/vendor.min.js'))
        .pipe(uglify({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('site.private.js', () => {
    return gulp
        .src([
            './wwwroot/scripts/shared/widgets/*.js',
            './wwwroot/scripts/shared/private.js'
        ])
        .pipe(concat('./wwwroot/scripts/Private/site.min.js'))
        .pipe(uglify({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('vendor.public.js', () => {
    return gulp
        .src([
            './wwwroot/scripts/jquery/jquery.js',
            './wwwroot/scripts/jquery/**/*.js',
            './wwwroot/scripts/bootstrap/*.js'
        ])
        .pipe(concat('./wwwroot/scripts/Public/vendor.min.js'))
        .pipe(uglify({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('site.public.js', () => {
    return gulp
        .src([
            './wwwroot/scripts/shared/widgets/validator.js',
            './wwwroot/scripts/shared/widgets/alerts.js',
            './wwwroot/scripts/shared/public.js'
        ])
        .pipe(concat('./wwwroot/scripts/Public/site.min.js'))
        .pipe(uglify({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('app.js', () => {
    return gulp
        .src(['./wwwroot/scripts/application/**/*.js', '!./wwwroot/scripts/application/**/*.min.js'])
        .pipe(uglify({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest(file => {
            file.basename = file.basename.split('.', 1)[0] + '.min.js';

            return file.base;
        }));
});

gulp.task('watch', () => {
    gulp.watch('./wwwroot/content/**/*.less', gulp.series([
        'less'
    ]));
});

gulp.task('minify', gulp.series([
    'clean.css',
    'clean.js',
    'less',
    'vendor.private.css',
    'vendor.public.css',
    'site.private.css',
    'site.public.css',
    'app.css',
    'vendor.private.js',
    'vendor.public.js',
    'site.private.js',
    'site.public.js',
    'app.js'
]));

gulp.task('default', gulp.series([
    'minify'
]));
