const path = require('path');
const webpack = require('webpack');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CompressionPlugin = require("compression-webpack-plugin");

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
            extensions: ['.js', '.jsx', '.ts', '.tsx']
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
                { test: /\.(png|woff|woff2|eot|ttf|svg)$/, use: ['url-loader?limit=100000'] },
                { test: /\.woff(\?\S*)?(\?v=[0-9]\.[0-9]\.[0-9])?$/, use: ['url-loader?limit=10000&mimetype=application/font-woff'] },
                { test: /\.(ttf|eot|svg)(\?\S*)(\?v=[0-9]\.[0-9]\.[0-9])?$/, use: ['file-loader'] }
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
            require('autoprefixer')
        ].concat(
            devMode ? [
            ] : [
                    new CompressionPlugin({
                        include: '/wwwroot/dist',
                        algorithm: "gzip",
                        test: /\.js$|\.css$|\.svg$/,
                        threshold: 10240,
                        minRatio: 0.8
                    })
                ])
    };

    return config;
};
