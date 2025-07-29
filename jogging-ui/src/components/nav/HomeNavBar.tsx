import { Button } from '@/components/ui/button';
import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuList,
} from '@/components/ui/navigation_menu';
import { Menu } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import AccountDropDown from '../dropdowns/AccountDropDown';
import { ModeToggle } from '@/components/ModeToggle';
import {
  Sheet,
  SheetClose,
  SheetContent,
  SheetTrigger,
} from '@/components/ui/sheet';

const HomeNavBar: React.FC = () => {
  const navigate = useNavigate();

  const navData = [
    { name: 'Wedstrijden', path: '/wedstrijden' },
    { name: 'Klassementen', path: '/klassementen' },
    { name: 'Loopclubs', path: '/loopclubs' },
    { name: 'Over ons', path: '/overons' },
    { name: 'Veelgestelde vragen', path: '/veelgesteldevragen' },
  ];

  const handleClick = (path: string) => {
    navigate(path);
  };

  return (
    <>
      {/* Mobile Navigation Drawer */}
      <div className='flex items-center justify-between w-full lg:hidden'>
        <Sheet>
          <SheetTrigger asChild>
            <Button variant='ghost' size='icon'>
              <Menu />
            </Button>
          </SheetTrigger>
          <SheetContent side='left'>
            <nav className='grid gap-y-6'>
              {navData.map((item) => (
                <SheetClose key={item.name} asChild>
                  <Button
                    variant='ghost'
                    onClick={() => handleClick(item.path)}
                  >
                    <p className='w-full text-lg font-medium text-left'>
                      {item.name}
                    </p>
                  </Button>
                </SheetClose>
              ))}
            </nav>
          </SheetContent>
        </Sheet>
        <div className='flex gap-3'>
          <ModeToggle />
          <AccountDropDown />
        </div>
      </div>

      {/* Desktop Navigation Bar */}
      <div className='flex items-center justify-center invisible gap-3 px-4 py-2 mx-auto rounded-full lg:visible w-max dark:bg-slate-700/50 bg-slate-50/50 backdrop-blur-sm'>
        <Link to='/' className='pb-0.5 font-bold'>
          Evergemse Joggings
        </Link>

        <NavigationMenu>
          <NavigationMenuList>
            {navData.map((item) => (
              <NavigationMenuItem key={item.name}>
                <Button
                  className='rounded-full'
                  variant='ghost'
                  onClick={() => handleClick(item.path)}
                >
                  {item.name}
                </Button>
              </NavigationMenuItem>
            ))}
          </NavigationMenuList>
        </NavigationMenu>

        <ModeToggle />
        <AccountDropDown />
      </div>
    </>
  );
};

export default HomeNavBar;
