export default {
  multipass: true,
  plugins: [
    {
      name: 'preset-default',
      params: {
        overrides: {
          removeViewBox: false,
          cleanupIDs: false,
          removeStyleElement: false,
          inlineStyles: false,
          collapseGroups: false,
        },
      },
    },
    {
      name: 'removeAttrs',
      params: {
        attrs: [
          'svg:xmlns:xlink',
          'svg:data-name',
          'xmlns:svg',
        ],
      },
    },
  ],
};
