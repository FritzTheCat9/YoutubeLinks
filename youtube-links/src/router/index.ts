import { createRouter, createWebHistory } from 'vue-router'
import HomePage from '../pages/HomePage.vue'
import TestView from '@/views/TestView.vue'
import PlaylistsPage from '@/pages/Playlists/PlaylistsPage.vue'
import DownloadLinkPage from '@/pages/Links/DownloadLinkPage.vue'
import UsersPage from '@/pages/Users/UsersPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomePage
    },
    {
      path: '/download-link',
      name: 'download-link',
      component: DownloadLinkPage
    },
    {
      path: '/users',
      name: 'users',
      component: UsersPage
    },
    {
      path: '/my-playlists',
      name: 'my-playlists',
      component: PlaylistsPage
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
    },
    {
      path: '/test',
      name: 'test',
      component: TestView
    },
  ]
})

export default router
