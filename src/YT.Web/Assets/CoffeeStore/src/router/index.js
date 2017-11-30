import Vue from "vue"
import Router from "vue-router"

Vue.use(Router)

export default new Router({
    routes: [{
            path: "/author",
            name: "author",
            component: r => require(["@/components/Author"], r)
        },
        {
            path: "/",
            name: "container",
            redirect: "/sign",
            component: r => require(["@/components/Container"], r),
            children: [{
                    path: "/sign",
                    name: "sign",
                    component: r => require(["@/components/Sign"], r)
                },
                {
                    path: "/accident",
                    name: "accident",
                    component: r => require(["@/components/Accident"], r)
                },
                {
                    path: "/detail",
                    name: "detail",
                    component: r => require(["@/components/Detail"], r)
                },
                {
                    path: "/map/:point",
                    name: "map",
                    component: r => require(["@/components/Map"], r)
                }
            ]

        }
    ]
})