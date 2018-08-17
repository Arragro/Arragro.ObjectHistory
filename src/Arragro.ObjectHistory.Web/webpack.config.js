const path = require('path');
const glob = require('glob-all');
const webpack = require('webpack');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CompressionPlugin = require("compression-webpack-plugin");
const TsConfigPathsPlugin = require('awesome-typescript-loader').TsConfigPathsPlugin;
const PurifyCSSPlugin = require('purifycss-webpack');

module.exports = (env) => {
    const devMode = env === null || env['run-prod'] === undefined || env['run-prod'] === null || env['run-prod'] === false;

    let config = {
        mode: devMode ? 'development' : 'production',
        devtool: 'source-map',
        resolve: {
            alias: {
                react: path.resolve(__dirname, './node_modules/react'),
                React: path.resolve(__dirname, './node_modules/react')
            },
            extensions: ['.js', '.jsx', '.ts', '.tsx'],
            plugins: [
                new TsConfigPathsPlugin(/* { tsconfig, compiler } */)
            ]
        },
        devServer: {
            hot: true
        },
        module: {
            rules: [
                {
                    test: /\.ts(x?)$/,
                    include: /ReactApp/,
                    exclude: [
                        /node_modules/,
                        path.resolve(__dirname, "../Arragro.ObjectHistory.Web.ReactAppLibrary/node_modules"),
                        /obj/
                    ],
                    loader: 'awesome-typescript-loader',
                    query: {
                        useBabel: true,
                        useCache: devMode
                    }
                },
                {
                    test: /\.(sa|sc|c)ss$/,
                    use: [
                        'css-hot-loader',
                        MiniCssExtractPlugin.loader,
                        {
                            loader: 'css-loader',
                            options: {
                                importLoaders: 2,
                                sourceMap: true
                            }
                        },
                        'postcss-loader',
                        {
                            loader: 'sass-loader',
                            options: {
                                sourceMap: true
                            }
                        }
                    ],
                },
                { test: /\.(png|woff|woff2|eot|ttf|svg)$/, loader: 'url-loader?limit=100000' },
                { test: /\.woff(\?\S*)?(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'url-loader?limit=10000&mimetype=application/font-woff' },
                { test: /\.(ttf|eot|svg)(\?\S*)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'file-loader' }
            ]
        },
        entry: {
            main: './ReactApp/index.tsx'
        },
        output: {
            path: path.join(__dirname, 'wwwroot', 'dist'),
            filename: '[name].js',
            chunkFilename: '[name].js',
            publicPath: '/dist/'
        },
        optimization: {
            splitChunks: {
                cacheGroups: {
                    styles: {
                        name: 'bootstrap',
                        test: /\.css$/,
                        chunks: 'all',
                        enforce: true
                    },
                    commons: {
                        test: /[\\/]node_modules[\\/]/,
                        name: "vendor",
                        chunks: "all"
                    }
                }
            }
        },
        plugins: [
            new MiniCssExtractPlugin({
                filename: "main.css",
                chunkFilename: "vendor.css"
            }),
            // new PurifyCSSPlugin({
            //     // Give paths to parse for rules. These should be absolute!
            //     paths: glob.sync([
            //         path.join(__dirname, './**/*.cshtml'),
            //         path.join(__dirname, './**/*.tsx')
            //     ]),
            //     purifyOptions: {
            //         info: true,
            //         minify: devMode,
            //         whitelist: [
            //             'dropdown-menu'
            //         ]
            //     }
            // }),
            require('autoprefixer'),
            new webpack.optimize.OccurrenceOrderPlugin()
        ].concat(
            devMode ? [
            ] : [
                    new CompressionPlugin({
                        asset: "[path].gz[query]",
                        //include: /\/wwwroot/,
                        algorithm: "gzip",
                        test: /\.js$|\.css$|\.svg$/,
                        threshold: 10240,
                        minRatio: 0.8
                    })
                ])
    };

    return config;
};
