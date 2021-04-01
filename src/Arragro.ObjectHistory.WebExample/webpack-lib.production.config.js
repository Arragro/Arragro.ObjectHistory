const path = require('path');
const webpack = require('webpack');
const TsconfigPathsPlugin = require('tsconfig-paths-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const rimraf = require('rimraf');
const { dependencies } = require('./package.json');

const libraryName = 'arragro-object-history';

rimraf.sync(path.join(__dirname, 'dist'));

module.exports = (env, argv) => {
    const mode = argv === undefined ? undefined : argv.mode;
    const devMode = mode === null || mode === undefined || mode === 'development';

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
                new TsconfigPathsPlugin({
                    configFile: path.resolve(__dirname, './tsconfig.lib.json'),
                    logLevel: 'info',
                    extensions: ['.ts', '.tsx']
                 })
            ]
        },
        module: {
            rules: [
                {
                    test: /\.ts(x?)$/,
                    include: /ReactApp/,
                    exclude: [
                        /node_modules/,
                        path.resolve(__dirname, "./node_modules"),
                        /obj/
                    ],        
                    use: {
                        loader: 'babel-loader',
                        options: {
                            cacheDirectory: true,
                            babelrc: false,
                            presets: [
                                [
                                    '@babel/preset-env',
                                    { 
                                        targets: { 
                                            browsers: 'last 2 versions'
                                        },
                                        modules: false
                                    }, // or whatever your project requires
                                ],
                                '@babel/preset-typescript',
                                '@babel/preset-react',
                            ],
                            plugins: [
                                // plugin-proposal-decorators is only needed if you're using experimental decorators in TypeScript
                                // ['@babel/plugin-proposal-decorators', { legacy: true }],
                                
                                ["@babel/plugin-transform-runtime", { "regenerator": true }],
                                ['@babel/plugin-proposal-class-properties', { loose: true }],
                                'lodash',
                                'syntax-dynamic-import',
                            ],
                        },
                    }
                },
                {
                    test: /\.(sa|sc|c)ss$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        {
                            loader: 'css-loader',
                            options: {
                                importLoaders: 2,
                                sourceMap: true
                            }
                        },
                        {
                            loader: 'sass-loader',
                            options: {
                                sourceMap: true
                            }
                        },
                        {
                            loader: 'postcss-loader',
                            options: {
                              postcssOptions: {
                                plugins: [
                                  [
                                    "autoprefixer",
                                    {
                                      // Options
                                    },
                                  ],
                                ],
                              },
                            },
                        },
                    ],
                },
                // { test: /\.(png|woff|woff2|eot|ttf|svg)$/, loader: 'url-loader?limit=100000' },
                // { test: /\.woff(\?\S*)?(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'url-loader?limit=10000&mimetype=application/font-woff' },
                // { test: /\.(ttf|eot|svg)(\?\S*)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'file-loader' },
                // { test: /\.json$/, loader: 'json-loader' }
            ]
        },
        entry: {
            library: ['./ReactAppLibrary/index.ts'],
        },
        externals: [
            'babel-polyfill',
            ...Object.keys(dependencies || {})
        ],
        output: {
            path: path.join(__dirname, 'dist'),
            libraryTarget: 'umd',
            library: libraryName,
            filename: `${libraryName}.js`
        },
        // optimization: {
        //     splitChunks: {
        //         cacheGroups: {
        //             styles: {
        //                 name: 'bootstrap',
        //                 test: /\.css$/,
        //                 chunks: 'all',
        //                 enforce: true
        //             },
        //             commons: {
        //                 test: /[\\/]node_modules[\\/]/,
        //                 name: "vendor",
        //                 chunks: "all"
        //             }
        //         }
        //     }
        // },
        plugins: [
            new MiniCssExtractPlugin(),
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
            // new webpack.optimize.OccurrenceOrderPlugin()
            // new webpack.optimize.CommonsChunkPlugin({ name: 'vendor', filename: 'vendor.bundle.js' }), // Moves vendor content out of other bundles
            // new BundleAnalyzerPlugin()
        ]
    }

    return config;
};