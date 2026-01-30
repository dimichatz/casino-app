import { Header, Footer } from "@/components/layouts/player";
import {Outlet} from "react-router";

const Layout = () => {
    return (
        <>
            <div className="bg-casinoapp-black flex flex-col">
                <header className="sticky top-0 h-16 z-40">
                    <Header />
                </header>


                <main className="flex-1 min-h-screen">
                    <Outlet />
                </main>

                <footer>
                    <Footer />
                </footer>

            </div>
        </>
    );
};

export default Layout;