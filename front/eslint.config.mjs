import { defineConfig, globalIgnores } from "eslint/config";
import nextVitals from "eslint-config-next/core-web-vitals";
import nextTs from "eslint-config-next/typescript";
import importPlugin from "eslint-plugin-import";

const eslintConfig = defineConfig([
  ...nextVitals,
  ...nextTs,

  {
    plugins: {
      import: importPlugin,
    },

    rules: {
      // Import organization
      "import/order": [
        "error",
        {
          groups: [
            "builtin",
            "external",
            "internal",
            "parent",
            "sibling",
            "index",
            "type",
          ],
          pathGroups: [
            {
              pattern: "@/**",
              group: "internal",
              position: "after",
            },
          ],
          pathGroupsExcludedImportTypes: ["builtin"],
          "newlines-between": "always",
          alphabetize: {
            order: "asc",
            caseInsensitive: true,
          },
        },
      ],

      // React Icons - MIT license only
      "no-restricted-imports": [
        "error",
        {
          patterns: [
            {
              regex:
                "^react-icons/(?!((ai|bi|bs|di|fc|fi|go|hi|hi2|io|io5|pi|rx|sl|tb|tfi)(/|$)))",
              message:
                "Somente bibliotecas de ícones React licenciadas sob a licença MIT são permitidas.",
            },
            {
              regex:
                "^@react-icons/all-files/(?!((ai|bi|bs|di|fc|fi|go|hi|hi2|io|io5|pi|rx|sl|tb|tfi)/))",
              message:
                "Somente bibliotecas de ícones React licenciadas sob a licença MIT são permitidas.",
            },
          ],
        },
      ],
    },
  },

  // Override default ignores of eslint-config-next.
  globalIgnores([".next/**", "out/**", "build/**", "next-env.d.ts"]),
]);

export default eslintConfig;
