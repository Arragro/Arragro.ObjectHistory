const webpack = require('webpack');
const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const { dependencies } = require('./package.json');

const dist = path.join(__dirname, 'wwwroot', 'dist', 'dll');

module.exports = (env) => {
    let config = {
        mode: 'development',
        devtool: 'source-map',
        module: {
            rules: [
                {
                    test: /\.js$/,
                    use: [ 'script-loader' ]
                }
            ]
        },

        entry: {
            vendor: [
                'babel-polyfill',
                ...Object.keys(dependencies || {})
            ]
                .filter(dependency => dependency !== 'bootstrap'),
            //.filter(dependency => dependency !== 'font-awesome'),
        },

        output: {
            library: '[name]_dll',
            path: dist,
            filename: '[name].dll.js',
            libraryTarget: 'var',
            sourceMapFilename: "[name].map",
            pathinfo: true
        },

        plugins: [
            new webpack.DllPlugin({
                path: path.join(dist, '[name]-manifest.json'),
                name: '[name]_dll',
            })
        ]
    };

    return config;
};
