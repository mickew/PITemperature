/// <binding Clean='clean' ProjectOpened='watch' />

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    less = require("gulp-less"),
    tsc = require("gulp-tsc"),
    rename = require("gulp-rename"),
    merge = require("merge-stream"),
    project = require("./project.json");

var paths = {
    bower:"./bower_components/",
    scripts: "./Scripts/",
    webroot: "./" + project.webroot + "/",
    css: "./" + project.webroot + "/css/",
    js: "./" + project.webroot + "/js/",
    fonts: "./" + project.webroot + "/fonts/",
    less: "Styles/*.less",
    ts: "ts/*.ts",
    concatCssDest: "./" + project.webroot + "/css/site.min.css"
};

var sources = {
    less: [
        {
            name: "site-less",
            path: paths.less
        }
    ],
    tsc: [
        {
            name: "typescript",
            path: paths.ts
        }
    ],
    mincss: [
        {
            name: "site-css",
            path: paths.css + "site.css"
        }
    ],
    minjs: [
        {
            name: "sensor.app.js",
            path: paths.js + "sensor.app.js"
        },
        {
            name: "temp.app.js",
            path: paths.js + "temp.app.js"
        }
    ],
    fonts: [
        {
            name: "bootstrap",
            path: paths.bower + "bootstrap/dist/**/*.{ttf,svg,woff,woff2,otf,eot}"
        },
        {
            name: "font-awesome",
            path: paths.bower + "font-awesome/**/*.{ttf,svg,woff,woff2,otf,eot}"
        },
        {
            name: "font-digital",
            path: paths.scripts + "canv-gauge/**/*.{ttf,svg,woff,woff2,otf,eot}"
        }
    ],
    css: [
        {
            name: "bootstrap",
            path: paths.bower + "bootstrap/dist/css/**/*.css"
        },
        {
            name: "font-awesome",
            path: paths.bower + "font-awesome/css/**/*.css"
        }
    ],
    js: [
        {
            name: "bootstrap",
            path: paths.bower + "bootstrap/dist/js/**/{bootstrap.js,bootstrap.min.js}"
        },
        {
            name: "jquery",
            path: paths.bower + "jquery/dist/**/{jquery.js,jquery.min.js}"
        },
        {
            name: "jquery-validate",
            path: paths.bower + "jquery-validation/dist/**/{jquery.validate.js,jquery.validate.min.js}"
        },
        {
            name: "jquery-validate-unobtrusive",
            path: paths.bower + "jquery-validation-unobtrusive/**/{jquery.validate.unobtrusive.js,jquery.validate.unobtrusive.min.js}"
        },
        {
            name: "knockout",
            path: paths.bower + "knockout/dist/**/{knockout.js,knockout.min.js}"
        },
        {
            name: "signalr",
            path: paths.bower + "signalr/**/{jquery.signalR.js,jquery.signalR.min.js}"
        },
        {
            name: "canv-gauge",
            path: paths.scripts + "canv-gauge/**/{gauge.js,gauge.min.js}"
        }
    ]
};

gulp.task("clean-fonts", function (cb) {
    return rimraf(paths.fonts + "*", cb);
});

gulp.task("clean-js", function (cb) {
    rimraf(paths.js + "*", cb);
});

gulp.task("clean-css", function (cb) {
    return rimraf(paths.css + "*", cb);
});

gulp.task("clean", ["clean-js", "clean-css", "clean-fonts"]);

gulp.task("less", function () {
    return gulp.src(paths.less)
    .pipe(less())
    .pipe(gulp.dest(paths.css))
});

gulp.task("tsc", function () {
    return gulp.src(paths.ts)
    .pipe(tsc({
        module: "CommonJS",
        sourcemap: false,
        emitError: true

    }))
    .pipe(gulp.dest(paths.js));
});

gulp.task("build-fonts", ["clean-fonts"], function () {
    var tasks = sources.fonts.map(function (source) { 
        return gulp                             
            .src(source.path)                   
            .pipe(rename(function (path) {      
                path.dirname = "";
            }))
            .pipe(gulp.dest(paths.fonts));      
    });
    //return merge(tasks);                      
});

gulp.task("build-css", ["clean-css"], function () {
    var tasks = sources.css.map(function (source) { 
        return gulp                             
            .src(source.path)                   
            .pipe(rename(function (path) {      
                path.dirname = "";
            }))
            .pipe(gulp.dest(paths.css));      
    });
    //return merge(tasks);
});

gulp.task("build-js", ["clean-js"], function () {
    var tasks = sources.js.map(function (source) { 
        return gulp                             
            .src(source.path)                   
            .pipe(rename(function (path) {      
                path.dirname = "";
            }))
            .pipe(gulp.dest(paths.js));      
    });
    //return merge(tasks);                   
});

gulp.task("build-less", function () {
    var tasks = sources.less.map(function (source) { 
        return gulp                             
            .src(source.path)                   
            .pipe(less())
            .pipe(gulp.dest(paths.webroot + 'css'))
    });
    //return merge(tasks);                      
});

gulp.task("build-min-css", function () {
    var tasks = sources.mincss.map(function (source) { 
        return gulp                             
            .src(source.path)                   
            .pipe(concat(paths.concatCssDest))
            .pipe(cssmin())
            .pipe(gulp.dest("."));
    });
    //return merge(tasks);                      
});

gulp.task("build-tsc", function () {
    var tasks = sources.tsc.map(function (source) { 
        return gulp                             
            .src(source.path)                   
            .pipe(tsc({
                module: "CommonJS",
                sourcemap: false,
                emitError: true

            }))
        .pipe(gulp.dest(paths.js));
    });
    //return merge(tasks);                        
});

gulp.task("build-min-js", function () {
    var tasks = sources.minjs.map(function (source) { 
        return gulp                             
            .src(source.path)                   
            .pipe(uglify())
            .pipe(rename({ extname: '.min.js' }))
            .pipe(gulp.dest(paths.js));
    });
    //return merge(tasks);                      
});

gulp.task("build", [
    "build-css",
    "build-fonts",
    "build-js",
    "build-less",
    "build-tsc",
    "build-min-css",
    "build-min-js"
]);

gulp.task('watch', function () {
    gulp.watch(paths.ts, ['tsc']);
    gulp.watch(paths.less, ['less']);
});