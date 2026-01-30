import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import "@/i18n";
import "@fortawesome/fontawesome-free/css/all.min.css";
import {PlayerProvider} from "@/context/PlayerProvider.tsx";
import {AuthProvider} from "@/context/AuthProvider.tsx";

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <AuthProvider>
            <PlayerProvider>
                <App />
            </PlayerProvider>
        </AuthProvider>
    </StrictMode>
)
