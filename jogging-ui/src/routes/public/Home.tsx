import { useEffect } from "react";
import heroFoto from "../../assets/images/heroFoto.jpg";

export default function Home() {
  return (
    <div className="flex flex-col items-center w-full min-h-screen">
      <div className="relative w-full h-full select-none -z-10">
        <img
          className="object-cover w-screen h-screen"
          src={heroFoto}
          alt=""
        />
        <div className="absolute top-0 flex items-end justify-center w-full h-full bg-gradient-to-b from-transparent to-background">
          <p className="p-6 font-semibold text-center mb-36 text-7xl md:text-8xl">
            Evergemse Joggings
          </p>
        </div>
      </div>
    </div>
  );
}
