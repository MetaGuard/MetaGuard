![](./Images/banner.png)

MetaGuard: Using Differential Privacy to go Incognito in the Metaverse

[![GitHub issues](https://img.shields.io/github/issues/MetaGuard/MetaGuard)](https://github.com/MetaGuard/MetaGuard/issues)
[![MIT](https://img.shields.io/badge/license-MIT-brightgreen.svg)](https://github.com/MetaGuard/MetaGuard/blob/master/LICENSE)

[Paper](https://arxiv.org/abs/2208.05604) |
[Website](https://rdi.berkeley.edu/metaguard/) |
[Main Script](https://github.com/MetaGuard/MetaGuard/blob/main/Assets/MetaGuard.cs) |
[Vivek Nair](https://github.com/VCNinc) |
[Gonzalo Munilla Garrido](https://github.com/gonzalo-munillag)

Virtual reality (VR) telepresence applications and the so-called "metaverse" promise to be the next major medium of interaction with the internet. However, with numerous recent studies showing the ease at which VR users can be profiled, deanonymized, and data harvested, metaverse platforms carry all the privacy risks of the current internet and more while at present having none of the defensive privacy tools we are accustomed to using on the web. To remedy this, we present the first known method of implementing an "incognito mode" for VR. Our technique leverages local ε-differential privacy to quantifiably obscure sensitive user data attributes, with a focus on intelligently adding noise when and where it is needed most to maximize privacy while minimizing usability impact. Moreover, our system is capable of flexibly adapting to the unique needs of each metaverse application to further optimize this trade-off. We implement our solution as a universal Unity (C#) plugin that we then evaluate using several popular VR applications. Upon faithfully replicating the most well known VR privacy attack studies, we show a significant degradation of attacker capabilities when using our proposed solution.

_We appreciate the support of the National Science Foundation, the National Physical Science Consortium, the Fannie and John Hertz Foundation, and the Berkeley Center for Responsible, Decentralized Intelligence._

[https://doi.org/10.48550/arXiv.2208.05604](https://doi.org/10.48550/arXiv.2208.05604)
