import React from "react";
import { Outlet } from "react-router-dom";
import NavMenu from "@/components/nav/HomeNavBar";
import Footer from "@/components/footer/Footer";

const RootLayout = () => {
  return (
    <div className="flex flex-col min-h-screen w-full">
      {/* Vastgezette header */}
      <header className="fixed top-0 z-40 w-full px-3 mt-4 md:px-6">
        <NavMenu />
      </header>

      {/* Inhoud */}
      <main className="flex-grow pt-[100px] pb-[140px] px-4">
        {/* 
          pt-[100px] => ruimte bovenaan voor de vaste header
          pb-[140px] => ruimte onderaan voor de vaste footer
        */}
        <Outlet />
      </main>

      {/* Vastgezette footer */}
      <footer className="fixed bottom-0 w-full z-30">
        <Footer />
      </footer>
    </div>
  );
};

export default RootLayout;

